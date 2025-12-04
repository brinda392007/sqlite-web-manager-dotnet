using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.UI;

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

                    if (contentType != "application/octet-stream")
                    {
                        lblStatus.Text = "Uploaded file is not a .db file.";
                        lblStatus.ForeColor = Color.Red;
                        return;
                    }

                    byte[] fileData;
                    using (BinaryReader br = new BinaryReader(fileUpload.PostedFile.InputStream))
                    {
                        fileData = br.ReadBytes(size);
                    }

                    int userId = Convert.ToInt32(Session["UserId"]);

                    SaveFileToDatabase(userId, filename, contentType, fileData, size);

                    lblStatus.Text = "Uploaded: " + filename;
                    lblStatus.ForeColor = Color.LightGreen;
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Upload Failed: " + ex.Message;
                    lblStatus.ForeColor = Color.Red;
                }
            }
            else
            {
                lblStatus.Text = "No file detected";
                lblStatus.ForeColor = Color.Yellow;
            }
        }

        protected void SaveFileToDatabase(int userId, string name, string type, byte[] data, int size)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("Uploads_CRUD", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EVENT", "INSERT");
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@FileName", name);
                    cmd.Parameters.AddWithValue("@ContentType", type);
                    cmd.Parameters.AddWithValue("@Content", data);
                    cmd.Parameters.AddWithValue("@Size", size);

                    // returns NewUploadId, if you ever need it:
                    object newUploadId = cmd.ExecuteScalar();
                }
            }
        }
    }
}
