using System;
using System.Data;
using System.Data.SqlClient;
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
                lblWelcome.Text = "Welcome, Operator " + Session["Username"];
                UiHelper.ShowSessionToast(this);  // shows login toast
            }

            LoadFiles();
            LoadGeneratedFiles();
        }

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

        protected void btnRefreshGenerated_Click(object sender, EventArgs e)
        {
            LoadGeneratedFiles();
        }



        protected void btnRefreshList_Click(object sender, EventArgs e)
        {
            LoadFiles();
        }

        protected void rptFiles_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteFile")
            {
                int fileId = Convert.ToInt32(e.CommandArgument);
                int userId = Convert.ToInt32(Session["UserId"]);

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

                        UiHelper.ShowToast(this, "File deleted successfully.", "success");
                    }
                }

                LoadFiles();
                LoadGeneratedFiles();

                UpdatePanelFiles.Update();
                UpdatePanelGenerated.Update();
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

                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();

                    // Optional: fetch FilePath and confirm owner (SELECT_BY_ID)
                    //using (var sel = new SqlCommand("GeneratedFiles_CRUD", conn))
                    //{
                    //    sel.CommandType = CommandType.StoredProcedure;
                    //    sel.Parameters.AddWithValue("@EVENT", "SELECT_BY_ID");
                    //    sel.Parameters.AddWithValue("@FileID", fileId);
                    //    sel.Parameters.AddWithValue("@UserId", userId);

                    //    using (var rdr = sel.ExecuteReader())
                    //    {
                    //        if (rdr.Read())
                    //        {
                    //            filePath = rdr["FilePath"]?.ToString();
                    //        }
                    //        else
                    //        {
                    //            UiHelper.ShowToast(this, "File not found or access denied.", "error");
                    //            LoadGeneratedFiles();
                    //            return;
                    //        }
                    //    }
                    //}

                    // Delete via stored procedure (SP will ensure ownership because we passed @UserId)
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
                catch
                {
                    // ignore file system errors (DB already cleaned). Optionally log them.
                }

                UiHelper.ShowToast(this, "Generated file purged.", "success");

                // Refresh UI
                LoadGeneratedFiles();

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
