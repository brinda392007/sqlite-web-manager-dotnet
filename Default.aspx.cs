using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Web;
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
                // This runs only once when the page first loads
                lblWelcome.Text = "Welcome, Operator " + Session["Username"];
            }
            LoadFiles();
        }
        private void LoadFiles()
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                string sql = "SELECT Id, FileName, Size, UploadedAt FROM Uploads WHERE UserId = @Uid ORDER BY UploadedAt DESC";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Uid", userId);

                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
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
            if(e.CommandName == "DeleteFile")
            {
                int fileId = Convert.ToInt32(e.CommandArgument);
                int userId = Convert.ToInt32(Session["UserId"]);

                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();

                    string sql = "DELETE FROM Uploads WHERE Id = @Fid AND UserId = @Uid";

                    using(var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Fid", fileId);
                        cmd.Parameters.AddWithValue("@Uid", userId);

                        cmd.ExecuteNonQuery();
                    }
                }

                LoadFiles();
            }
        }

        public string FormatSize(object sizeObj)
        {
            long bytes = Convert.ToInt64(sizeObj);
            if (bytes >= 1024) return (bytes / 1024f).ToString("0.00") + " KB";
            if (bytes >= 1048576) return (bytes / 1024f / 1024f).ToString("0.00") + " MB";
            return bytes + " B";
        }
    }
}