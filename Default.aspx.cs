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
