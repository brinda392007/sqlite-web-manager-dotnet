using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace ASPWeBSM
{
    public class GeneratedDownload : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Session["UserId"] == null)
            {
                context.Response.StatusCode = 403;
                context.Response.End();
                return;
            }

            string fileIdStr = context.Request.QueryString["id"];
            if (!int.TryParse(fileIdStr, out int fileId))
            {
                context.Response.StatusCode = 400;
                context.Response.End();
                return;
            }

            int userId = Convert.ToInt32(context.Session["UserId"]);

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("GeneratedFiles_CRUD", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EVENT", "SELECT_BY_ID");
                    cmd.Parameters.AddWithValue("@FileID", fileId);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (!rdr.Read())
                        {
                            context.Response.StatusCode = 404;
                            context.Response.End();
                            return;
                        }

                        string fileName = rdr["FileName"].ToString();
                        string filePath = rdr["FilePath"].ToString();
                        string physical = context.Server.MapPath(filePath);

                        if (!File.Exists(physical))
                        {
                            context.Response.StatusCode = 404;
                            context.Response.End();
                            return;
                        }

                        context.Response.Clear();
                        context.Response.ContentType = "application/octet-stream";
                        context.Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
                        context.Response.WriteFile(physical);
                        LogManager.Success($"Generated file downloaded: {fileName}");
                        context.Response.End();
                        return;
                    }
                }
            }
        }

        public bool IsReusable => false;
    }
}
