using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASPWeBSM
{
    public partial class Upload : AuthorizedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                try
                {
                    string filename = fileUpload.FileName;
                    string contentType = fileUpload.PostedFile.ContentType;
                    int size = fileUpload.PostedFile.ContentLength;

                    if(contentType != "application/octet-stream")
                    {
                        lblStatus.Text = "Uploaded file not a Db file";
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    byte[] fileData;
                    using(BinaryReader br = new BinaryReader(fileUpload.PostedFile.InputStream))
                    {
                        fileData = br.ReadBytes(size);
                    }

                    int userId = Convert.ToInt32(Session["UserId"]);

                    SaveFileToDatabase(userId, filename, contentType, fileData, size);

                    lblStatus.Text = "Uploaded: " + filename;
                    lblStatus.ForeColor = System.Drawing.Color.LightGreen;
                }
                catch(Exception ex)
                {
                    lblStatus.Text = "Upload Failed: " + ex.Message;
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                lblStatus.Text = "No file detected";
                lblStatus.ForeColor = System.Drawing.Color.Yellow;
            }
        }

        protected void SaveFileToDatabase(int UserId, string name, string type, byte[] data, int size)
        {
            using(var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                string sql = @"INSERT INTO Uploads(UserId, FileName, ContentType, Content, Size)
                                Values (@UserId, @Name, @Type, @Data, @Size)";

                using(var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("Type", type);
                    cmd.Parameters.AddWithValue("@Data", data);
                    cmd.Parameters.AddWithValue("@Size", size);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}