using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.SessionState;

namespace ASPWeBSM
{
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

            string fileIdStr = context.Request.QueryString["id"];
            int userId = Convert.ToInt32(context.Session["UserId"]);

            if (!string.IsNullOrEmpty(fileIdStr))
            {
                int fileId = Convert.ToInt32(fileIdStr);

                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();

                    using (var cmd = new SqlCommand("Uploads_CRUD", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@EVENT", "SELECT_BY_ID");
                        cmd.Parameters.AddWithValue("@Id", fileId);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
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
