using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASPWeBSM
{
    public partial class Analyze : AuthorizedPage
    {
        // simple class to hold operations per table
        private class TableOperationSelection
        {
            public string TableName { get; set; }
            public bool Select { get; set; }
            public bool Insert { get; set; }
            public bool Update { get; set; }
            public bool Delete { get; set; }
            public bool SelectById { get; set; }
            public bool SelectAll { get; set; }
        }

        private class ColumnInfo
        {
            public string Name { get; set; }
            public string SqlType { get; set; }      // SQL Server type
            public bool IsPrimaryKey { get; set; }
        }

        protected int UploadId
        {
            get
            {
                int id;
                int.TryParse(Request.QueryString["uploadId"], out id);
                return id;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (UploadId <= 0)
                {
                    lblMessage.Text = "Invalid upload id.";
                    pnlTables.Visible = false;
                    return;
                }

                try
                {
                    LoadTablesForUpload(UploadId);
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error loading tables: " + ex.Message;
                    pnlTables.Visible = false;
                }
            }
        }

        private void LoadTablesForUpload(int uploadId)
        {
            // 1. Get uploaded file from SQL Server
            string fileName;
            byte[] content;

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("Uploads_CRUD", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EVENT", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@Id", uploadId);
                    cmd.Parameters.AddWithValue("@UserId", Convert.ToInt32(Session["UserId"]));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new Exception("Upload not found or access denied.");
                        }

                        fileName = reader["FileName"].ToString();
                        content = (byte[])reader["Content"];
                    }
                }
            }

            lblDbName.Text = "Analyzing: " + fileName;

            // 2. Write BLOB to temp .db file
            string tempFolder = Server.MapPath("~/App_Data/Temp");
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            string tempPath = Path.Combine(tempFolder, $"upload_{uploadId}_{fileName}");
            File.WriteAllBytes(tempPath, content);

            // store for later (SP/method generation)
            ViewState["CurrentDbPath"] = tempPath;

            // 3. Read table list using SQLite
            DataTable dtTables = new DataTable();
            dtTables.Columns.Add("TableName", typeof(string));

            string sqliteConnStr = $"Data Source={tempPath};Version=3;";
            using (var sqliteConn = new SQLiteConnection(sqliteConnStr))
            {
                sqliteConn.Open();

                string sql = "SELECT name FROM sqlite_master WHERE type='table' " +
                            "AND name NOT LIKE 'sqlite_%' ORDER BY name";

                using (var cmd = new SQLiteCommand(sql, sqliteConn))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tblName = reader["name"].ToString();
                        dtTables.Rows.Add(tblName);
                    }
                }
            }

            if (dtTables.Rows.Count == 0)
            {
                lblMessage.Text = "No user tables found in this database.";
            }

            rptTables.DataSource = dtTables;
            rptTables.DataBind();
        }

        private string GetCurrentDbPath()
        {
            return ViewState["CurrentDbPath"] as string;
        }

        private List<TableOperationSelection> GetSelectedOperations()
        {
            var list = new List<TableOperationSelection>();

            foreach (RepeaterItem item in rptTables.Items)
            {
                var hfTableName = (HiddenField)item.FindControl("hfTableName");
                var chkSelect = (CheckBox)item.FindControl("chkSelect");
                var chkInsert = (CheckBox)item.FindControl("chkInsert");
                var chkUpdate = (CheckBox)item.FindControl("chkUpdate");
                var chkDelete = (CheckBox)item.FindControl("chkDelete");
                var chkSelectById = (CheckBox)item.FindControl("chkSelectById");
                var chkSelectAll = (CheckBox)item.FindControl("chkSelectAll");

                if (hfTableName == null) continue;

                if (chkSelectAll.Checked)
                {
                    list.Add(new TableOperationSelection
                    {
                        TableName = hfTableName.Value,
                        Select = true,
                        Insert = true,
                        Update = true,
                        Delete = true,
                        SelectById = true,
                    });
                    continue;
                }

                if (chkSelect.Checked || chkInsert.Checked || chkUpdate.Checked ||
                chkDelete.Checked || chkSelectById.Checked)
                {
                    list.Add(new TableOperationSelection
                    {
                        TableName = hfTableName.Value,
                        Select = chkSelect.Checked,
                        Insert = chkInsert.Checked,
                        Update = chkUpdate.Checked,
                        Delete = chkDelete.Checked,
                        SelectById = chkSelectById.Checked
                    });
                }
            }

            return list;
        }

        private string BuildOperationsSummary(List<TableOperationSelection> selections)
        {
            var parts = new List<string>();

            foreach (var s in selections)
            {
                var ops = new List<string>();
                if (s.Select) ops.Add("SELECT");
                if (s.Insert) ops.Add("INSERT");
                if (s.Update) ops.Add("UPDATE");
                if (s.Delete) ops.Add("DELETE");
                if (s.SelectById) ops.Add("SELECT_BY_ID");

                parts.Add($"{s.TableName}: {string.Join(", ", ops)}");
            }

            return string.Join(" | ", parts);
        }

        private string SaveGeneratedFile(string content, string operationsInfo, string kind)
        {
            string genFolder = Server.MapPath("~/App_Data/Generated");
            if (!Directory.Exists(genFolder))
                Directory.CreateDirectory(genFolder);

            string fileName = $"Generated_{UploadId}_{kind}_{DateTime.Now:yyyyMMddHHmmss}.txt";
            string physicalPath = Path.Combine(genFolder, fileName);
            File.WriteAllText(physicalPath, content);

            // store metadata in GeneratedFiles table
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("GeneratedFiles_CRUD", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EVENT", "INSERT");
                    cmd.Parameters.AddWithValue("@UserId", Convert.ToInt32(Session["UserId"]));
                    cmd.Parameters.AddWithValue("@UploadId", UploadId);
                    cmd.Parameters.AddWithValue("@FileName", fileName);
                    cmd.Parameters.AddWithValue("@FilePath", "~/App_Data/Generated/" + fileName);
                    cmd.Parameters.AddWithValue("@OperationsInfo", operationsInfo + $" ({kind})");

                    cmd.ExecuteScalar(); // returns new id, we don't need it here
                }
            }

            return fileName;
        }

        protected void btnGenerateSp_OnClick(object sender, EventArgs e)
        {
            var selections = GetSelectedOperations();
            if (selections.Count == 0)
            {
                lblMessage.Text = "Select at least one operation for any table.";
                return;
            }

            string dbPath = GetCurrentDbPath();
            if (string.IsNullOrEmpty(dbPath))
            {
                lblMessage.Text = "Session expired. Please reload this page.";
                return;
            }

            string spText = GenerateSpScript(selections, dbPath);
            string summary = BuildOperationsSummary(selections);
            string fileName = SaveGeneratedFile(spText, summary, "SP");

            lblMessage.Text = $"Stored procedures generated in file: {fileName}. You can download it later from your downloads panel.";
            Response.Redirect("Default.aspx");
        }

        protected void btnGenerateMethods_OnClick(object sender, EventArgs e)
        {
            var selections = GetSelectedOperations();
            if (selections.Count == 0)
            {
                lblMessage.Text = "Select at least one operation for any table.";
                return;
            }

            string dbPath = GetCurrentDbPath();
            if (string.IsNullOrEmpty(dbPath))
            {
                lblMessage.Text = "Session expired. Please reload this page.";
                return;
            }

            string methodsText = GenerateMethodsScript(selections, dbPath);
            string summary = BuildOperationsSummary(selections);
            string fileName = SaveGeneratedFile(methodsText, summary, "Methods");

            lblMessage.Text = $"C# methods generated in file: {fileName}. You can download it later from your downloads panel.";
            Response.Redirect("Default.aspx");
        }

        protected void btnGenerateBoth_OnClick(object sender, EventArgs e)
        {
            var selections = GetSelectedOperations();
            if (selections.Count == 0)
            {
                lblMessage.Text = "Select at least one operation for any table.";
                return;
            }

            string dbPath = GetCurrentDbPath();
            if (string.IsNullOrEmpty(dbPath))
            {
                lblMessage.Text = "Session expired. Please reload this page.";
                return;
            }

            string spText = GenerateSpScript(selections, dbPath);
            string methodsText = GenerateMethodsScript(selections, dbPath);

            var full = new StringBuilder();
            full.AppendLine("-- =============================================");
            full.AppendLine("-- STORED PROCEDURES");
            full.AppendLine("-- =============================================");
            full.AppendLine();
            full.AppendLine(spText);
            full.AppendLine();
            full.AppendLine("// =============================================");
            full.AppendLine("// C# METHODS");
            full.AppendLine("// =============================================");
            full.AppendLine();
            full.AppendLine(methodsText);

            string summary = BuildOperationsSummary(selections);
            string fileName = SaveGeneratedFile(full.ToString(), summary, "SP_AND_Methods");

            lblMessage.Text = $"SP + C# methods generated in file: {fileName}. You can download it later from your downloads panel.";
            Response.Redirect("Default.aspx");
        }

        //handles the Select all button
        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            //get the select all square that was checked
            CheckBox chkAll = (CheckBox)sender;

            //find the specific row
            RepeaterItem item = (RepeaterItem)chkAll.NamingContainer;

            CheckBox select = (CheckBox)item.FindControl("chkSelect");
            CheckBox insert= (CheckBox)item.FindControl("chkInsert");
            CheckBox update = (CheckBox)item.FindControl("chkUpdate");
            CheckBox delete = (CheckBox)item.FindControl("chkDelete");
            CheckBox selectById = (CheckBox)item.FindControl("chkSelectById");

            bool isChecked = chkAll.Checked;

            if (select != null) select.Checked = isChecked;
            if (insert != null) insert.Checked = isChecked;
            if (update != null) update.Checked = isChecked;
            if (delete != null) delete.Checked = isChecked;
            if (selectById != null) selectById.Checked = isChecked;
        }
    }
}