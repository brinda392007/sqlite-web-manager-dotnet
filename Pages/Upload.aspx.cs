using System;
using System.Collections.Generic;
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
            UiHelper.ShowSessionToast(this);
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                try
                {
                    List<string> allowedExtentions = new List<string> { ".db", ".sql" };
                    string filename = fileUpload.FileName;
                    string contentType = fileUpload.PostedFile.ContentType;
                    int size = fileUpload.PostedFile.ContentLength;
                    string ext = Path.GetExtension(fileUpload.FileName);

                    if (!allowedExtentions.Contains(ext))
                    {
                        lblStatus.Text = "Uploaded file is not a .db or .sql file.";
                        lblStatus.ForeColor = Color.Red;
                        LogManager.Error($"It is not .db or .sql file");
                        UiHelper.ShowToast(this, "Please upload a .db or .sql file.", "error");

                        return;
                    }

                    byte[] fileData;
                    using (BinaryReader br = new BinaryReader(fileUpload.PostedFile.InputStream))
                    {
                        fileData = br.ReadBytes(size);
                    }

                    int userId = Convert.ToInt32(Session["UserId"]);

                    int? newUploadId = SaveFileToDatabase(userId, filename, contentType, fileData, size);

                    if (newUploadId.HasValue)
                    {
                        LogManager.Success($"Uploaded file '{filename}' successfully.");
                        UiHelper.SetToast("File uploaded successfully.", "success");

                        string url = $"Analyze.aspx?uploadId={newUploadId}";

                        Response.Redirect(url, false);
                    }
                    else
                    {
                        UiHelper.SetToast("Server Did not return an upload id", "info");
                        LogManager.Error($"Warning server did not return an upload id for the last file upload.");
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Upload Failed: " + ex.Message;
                    lblStatus.ForeColor = Color.Red;
                    LogManager.Error($"Uploade fail.");
                    UiHelper.ShowToast(this, "Upload failed: " + ex.Message, "error");
                }
            }
            else
            {
                lblStatus.Text = "No file detected";
                lblStatus.ForeColor = Color.Yellow;

                UiHelper.ShowToast(this, "No file selected.", "error");
            }
        }

        protected int? SaveFileToDatabase(int userId, string name, string type, byte[] data, int size)
        {
            DatabaseManager.Initialize();
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

                    // returns NewUploadId
                    object result = cmd.ExecuteScalar();

                    if(result == null || result == DBNull.Value)
                    {
                        return null;
                    }

                    if(int.TryParse(result.ToString(), out int id))
                    {
                        return id;
                    }

                    try
                    {
                        return Convert.ToInt32(result);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }
    }
}
