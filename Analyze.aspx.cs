using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
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

            // 3. Read table list using SQLite
            DataTable dtTables = new DataTable();
            dtTables.Columns.Add("TableName", typeof(string));

            string sqliteConnStr = $"Data Source={tempPath};Version=3;";
            using (var sqliteConn = new SQLiteConnection(sqliteConnStr))
            {
                sqliteConn.Open();

                string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name";

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

                if (hfTableName == null) continue;

                // only add if at least one operation is selected
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

        protected void btnGenerateSp_OnClick(object sender, EventArgs e)
        {
            var selections = GetSelectedOperations();
            if (selections.Count == 0)
            {
                lblMessage.Text = "Select at least one operation for any table.";
                return;
            }

            // 🔜 Next step: implement real SP generation here
            // For now, just show a message with count
            lblMessage.Text = $"[DEBUG] Will generate SPs for {selections.Count} table(s).";
        }

        protected void btnGenerateMethods_OnClick(object sender, EventArgs e)
        {
            var selections = GetSelectedOperations();
            if (selections.Count == 0)
            {
                lblMessage.Text = "Select at least one operation for any table.";
                return;
            }

            lblMessage.Text = $"[DEBUG] Will generate method code for {selections.Count} table(s).";
        }

        protected void btnGenerateBoth_OnClick(object sender, EventArgs e)
        {
            var selections = GetSelectedOperations();
            if (selections.Count == 0)
            {
                lblMessage.Text = "Select at least one operation for any table.";
                return;
            }

            lblMessage.Text = $"[DEBUG] Will generate SPs + methods for {selections.Count} table(s).";
        }
    }
}
