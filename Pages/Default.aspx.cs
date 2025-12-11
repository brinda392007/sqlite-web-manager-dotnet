using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;
using System.Web; // Needed for HttpUtility.HtmlEncode
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics; // Added for the Debug.WriteLine diagnostic

namespace ASPWeBSM
{
    public partial class Default : AuthorizedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Assuming you have a label named lblWelcome
                if (Session["Username"] != null)
                {
                    lblWelcome.Text = "Welcome, Operator " + Session["Username"];
                }

                UiHelper.ShowSessionToast(this); // shows login toast
            }

            LoadFiles();
            LoadGeneratedFiles();

            // CRITICAL CALL: Load logs on every page load
            GetLogsHtml();
        }

        private void GetLogsHtml()
        {
            var logHtml = new StringBuilder();
            logHtml.AppendLine("<div class=\"log-entry text-sm text-slate-400\"><span class=\"text-orange-400\">[Time]</span> (Level) Message...</div><hr class=\"border-slate-700 my-1\">");

            try
            {
                // Use the dedicated SQLite connection
                using (var conn = DatabaseManager.GetLogConnection())
                {
                    conn.Open();

                    // SQLITE QUERY: Standard log retrieval
                    string sql = @"
SELECT 
    LogTime,
    Message,
    LogLevel
FROM 
    Logs 
ORDER BY 
    LogTime DESC
LIMIT 50;";

                    using (var cmd = new System.Data.SQLite.SQLiteCommand(sql, conn)) // Use SQLiteCommand
                    {
                        using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // 1. Read and process data
                                string timestamp = Convert.ToDateTime(reader["LogTime"]).ToString("HH:mm:ss");
                                string level = reader["LogLevel"].ToString().ToLowerInvariant();
                                string message = reader["Message"].ToString();
                                string username = "SYSTEM"; // Hardcoded username

                                // 2. Determine CSS class based on log level
                                string colorClass = "text-slate-400"; // Default (e.g., for DEBUG)

                                if (level == "error")
                                {
                                    colorClass = "text-red-400"; // Red for ERROR
                                }
                                else if (level == "success") // Check for SUCCESS
                                {
                                    colorClass = "text-emerald-400"; // Green for SUCCESS
                                }
                                else if (level == "info") // Check for INFO
                                {
                                    colorClass = "text-yellow-400"; // Yellow/Orange for INFO
                                }

                                // Diagnostics (Optional, but useful to see the read level)
                                // Debug.WriteLine($"Log Level: {level}, Color: {colorClass}");


                                // 3. Build the HTML for one log entry
                                logHtml.AppendLine($"<div class=\"log-entry text-sm\">");
                                logHtml.AppendLine($"<span class=\"{colorClass}\">[{timestamp}]</span> ");
                                logHtml.AppendLine($"<span class=\"text-slate-500\">({username})</span>: ");
                                // FIX APPLIED HERE: Using {colorClass} for the message text
                                logHtml.AppendLine($"<span class=\"{colorClass}\">{HttpUtility.HtmlEncode(message)}</span>");
                                logHtml.AppendLine($"</div>");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving SQLite logs: {ex.Message}");
                logHtml.AppendLine("<div class=\"log-entry text-red-500 text-sm\">[ERROR] Failed to load log history.</div>");
            }

            // Inject the generated HTML
            string script = $"document.getElementById('log-panel-content').innerHTML = \"{logHtml.ToString().Replace("\"", "\\\"").Replace("\r\n", "")}\";";
            ScriptManager.RegisterStartupScript(this, GetType(), "LoadLogsScript", script, true);
        }

        private void LoadFiles()
        {
            // ... (Your LoadFiles implementation) ...
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

        private void LoadGeneratedFiles()
        {
            // ... (Your LoadGeneratedFiles implementation) ...
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

        protected void btnRefreshGenerated_Click(object sender, EventArgs e)
        {
            LoadGeneratedFiles();
            GetLogsHtml();
        }


        protected void btnRefreshList_Click(object sender, EventArgs e)
        {
            LoadFiles();
            GetLogsHtml();
        }

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

                            // Log successful deletion
                            // IMPORTANT: Delete is a completed action, should be SUCCESS.
                            LogManager.Success($"User {Session["Username"]} deleted uploaded file ID: {fileId}.");
                            UiHelper.ShowToast(this, "File deleted successfully.", "success");
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error deleting uploaded file ID: {fileId}.", ex);
                    UiHelper.ShowToast(this, "Error deleting file.", "error");
                }


                LoadFiles();
                LoadGeneratedFiles();
                GetLogsHtml(); // Refresh logs after an action

                // Assuming these UpdatePanels exist
                // UpdatePanelFiles.Update();
                // UpdatePanelGenerated.Update();
            }
        }

        protected void rptGenerated_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteFile")
            {
                int fileId;
                if (!int.TryParse(e.CommandArgument?.ToString(), out fileId))
                    return;

                int userId = Convert.ToInt32(Session["UserId"]);
                string filePath = null;

                try
                {
                    using (var conn = DatabaseManager.GetConnection())
                    {
                        conn.Open();

                        // Delete via stored procedure
                        using (var cmd = new SqlCommand("GeneratedFiles_CRUD", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@EVENT", "DELETE");
                            cmd.Parameters.AddWithValue("@FileID", fileId);
                            cmd.Parameters.AddWithValue("@UserId", userId);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Delete physical file (best-effort)
                    try
                    {
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            string physical = Server.MapPath(filePath);
                            if (System.IO.File.Exists(physical))
                                System.IO.File.Delete(physical);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log file system errors (DB cleaned, but file remained)
                        LogManager.Error($"Filesystem error deleting generated file ID: {fileId} at {filePath}", ex);
                    }

                    // Log successful deletion
                    // IMPORTANT: Purge is a completed action, should be SUCCESS.
                    LogManager.Success($"User {Session["Username"]} purged generated file ID: {fileId}.");
                    UiHelper.ShowToast(this, "Generated file purged.", "success");
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error purging generated file ID: {fileId}.", ex);
                    UiHelper.ShowToast(this, "Error purging generated file.", "error");
                }

                // Refresh UI
                LoadGeneratedFiles();
                GetLogsHtml(); // Refresh logs after an action
            }
        }

        public string FormatSize(object sizeObj)
        {
            long bytes = Convert.ToInt64(sizeObj);

            if (bytes >= 1048576) // 1 MB
                return (bytes / 1024f / 1024f).ToString("0.00") + " MB";
            if (bytes >= 1024) // 1 KB
                return (bytes / 1024f).ToString("0.00") + " KB";

            return bytes + " B";
        }
    }
}