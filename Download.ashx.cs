using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.SessionState;

namespace ASPWeBSM
{
    /// <summary>
    /// Summary description for Download
    /// </summary>
    public class Download : IHttpHandler, IReadOnlySessionState
    {

        public void ProcessRequest(HttpContext context)
        {

            if (context.Session["UserId"] == null)
            {
                context.Response.StatusCode = 403;
                context.Response.End();
                return;
            }

            string fileId = context.Request.QueryString["id"];
            int userId = Convert.ToInt32(context.Session["UserId"]);

            if (!String.IsNullOrEmpty(fileId))
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();

                    string sql = "SELECT FileName, ContentType, Content FROM Uploads WHERE Id = @Fid AND UserId = @Uid";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Fid", fileId);
                        cmd.Parameters.AddWithValue("@Uid", userId);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fileName = reader["FileName"].ToString();
                                string contentType = reader["ContentType"].ToString();
                                byte[] fileData = (byte[])reader["Content"];

                                context.Response.Clear();
                                context.Response.ContentType = contentType;
                                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                                context.Response.BinaryWrite(fileData);
                                context.Response.End();
                            }
                        }
                    }
                }
            }
        }
        public bool IsReusable => false;
    }
}