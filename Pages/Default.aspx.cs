using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASPWeBSM
{
    public partial class Default : AuthorizedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Username"] != null)
                {
                    lblWelcome.Text = "Welcome, Operator " + Session["Username"];
                }

                UiHelper.ShowSessionToast(this);

                LoadFiles();
                LoadGeneratedFiles();
                LoadLogs();
            }
        }

        // =========================
        // LOGS (CLEAN + SIMPLE)
        // =========================
        private void LoadLogs()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            DataTable dt = new DataTable();

            dt.Columns.Add("Time");
            dt.Columns.Add("Message");
            dt.Columns.Add("ColorClass");

            using (var conn = DatabaseManager.GetLogConnection())
            {
                conn.Open();

                string sql = @"
SELECT LogTime, LogType, Message
FROM Logs
WHERE UserId = @UserId
ORDER BY LogTime DESC
LIMIT 50;";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataRow row = dt.NewRow();

                            string level = reader["LogType"].ToString().ToUpperInvariant();
                            DateTime utcTime = Convert.ToDateTime(reader["LogTime"]);
                            DateTime localTime = utcTime.ToLocalTime();
                            row["Time"] = localTime.ToString("HH:mm:ss");
                            row["Message"] = reader["Message"].ToString();

                            switch (level)
                            {
                                case "SUCCESS":
                                    row["ColorClass"] = "text-emerald-400";
                                    break;
                                case "ERROR":
                                    row["ColorClass"] = "text-red-400";
                                    break;
                                case "INFO":
                                    row["ColorClass"] = "text-yellow-400";
                                    break;
                                default:
                                    row["ColorClass"] = "text-slate-400";
                                    break;
                            }

                            dt.Rows.Add(row);
                        }
                    }
                }
            }

            rptLogs.DataSource = dt;
            rptLogs.DataBind();
            lblNoLogs.Visible = (dt.Rows.Count == 0);
        }

        // =========================
        // UPLOADS
        // =========================
        private void LoadFiles()
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("Uploads_CRUD", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EVENT", "SELECT");
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        rptFiles.DataSource = dt;
                        rptFiles.DataBind();

                        lblEmpty.Visible = (dt.Rows.Count == 0);
                    }
                }
            }
        }

        // =========================
        // GENERATED FILES
        // =========================
        private void LoadGeneratedFiles()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            DataTable dt = new DataTable();

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("GeneratedFiles_CRUD", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EVENT", "SELECT");
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            rptGenerated.DataSource = dt;
            rptGenerated.DataBind();
            lblGeneratedEmpty.Visible = (dt.Rows.Count == 0);
        }

        // =========================
        // BUTTONS
        // =========================
        protected void btnRefreshList_Click(object sender, EventArgs e)
        {
            LoadFiles();
            LoadLogs();
        }

        protected void btnRefreshGenerated_Click(object sender, EventArgs e)
        {
            LoadGeneratedFiles();
            LoadLogs();
        }

        // =========================
        // DELETE UPLOAD
        // =========================
        protected void rptFiles_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteFile")
            {
                int fileId = Convert.ToInt32(e.CommandArgument);
                int userId = Convert.ToInt32(Session["UserId"]);

                try
                {
                    using (var conn = DatabaseManager.GetConnection())
                    {
                        conn.Open();

                        using (var cmd = new SqlCommand("Uploads_CRUD", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@EVENT", "DELETE");
                            cmd.Parameters.AddWithValue("@Id", fileId);
                            cmd.Parameters.AddWithValue("@UserId", userId);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    LogManager.Success("Uploaded file deleted.");
                    UiHelper.ShowToast(this, "File deleted successfully.", "success");
                }
                catch (Exception ex)
                {
                    LogManager.Error("Error deleting uploaded file. "+ ex);
                    UiHelper.ShowToast(this, "Error deleting file.", "error");
                }

                LoadFiles();
                LoadGeneratedFiles();
                LoadLogs();
                
            }
        }

        // =========================
        // DELETE GENERATED FILE
        // =========================
        protected void rptGenerated_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteFile")
            {
                int fileId = Convert.ToInt32(e.CommandArgument);
                int userId = Convert.ToInt32(Session["UserId"]);

                try
                {
                    using (var conn = DatabaseManager.GetConnection())
                    {
                        conn.Open();

                        using (var cmd = new SqlCommand("GeneratedFiles_CRUD", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@EVENT", "DELETE");
                            cmd.Parameters.AddWithValue("@FileID", fileId);
                            cmd.Parameters.AddWithValue("@UserId", userId);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    LogManager.Success("Generated file purged.");
                    UiHelper.ShowToast(this, "Generated file purged.", "success");
                }
                catch (Exception ex)
                {
                    LogManager.Error("Error purging generated file. " + ex);
                    UiHelper.ShowToast(this, "Error purging generated file.", "error");
                }

                LoadGeneratedFiles();
                LoadLogs();
                upLogs.Update();
            }
        }

        // =========================
        // SIZE FORMAT
        // =========================
        public string FormatSize(object sizeObj)
        {
            long bytes = Convert.ToInt64(sizeObj);

            if (bytes >= 1048576)
                return (bytes / 1024f / 1024f).ToString("0.00") + " MB";
            if (bytes >= 1024)
                return (bytes / 1024f).ToString("0.00") + " KB";

            return bytes + " B";
        }
    }
}
