using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASPWeBSM
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔴 Hide logout button
            Page.Master.FindControl("btnLogOut").Visible = false;

            // 🔴 Prevent caching
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));

            // ✅ If already logged in → redirect to Home
            if (Session["UserId"] != null)
            {
                Response.Redirect("~/Pages/Default.aspx");
                return;
            }

            UiHelper.ShowSessionToast(this);
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string name = txtUser.Text; // This variable holds the username
            string pass = txtPassword.Text;

            DatabaseManager.Initialize();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("Users_CRUD", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EVENT", "LOGIN");
                    cmd.Parameters.AddWithValue("@Username", name);
                    cmd.Parameters.AddWithValue("@Password", pass);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // SUCCESSFUL LOGIN BLOCK
                            Session["UserId"] = reader["Id"];
                            Session["Username"] = reader["Username"];

                            UiHelper.SetToast("Login successful", "success");

                            // *** CORRECT LOG CALL: Calls LogManager.Success (Green) ***
                            LogManager.Success($"logged in successfully.");

                            Response.Redirect("Default.aspx");
                        }
                        else
                        {
                            // FAILED LOGIN BLOCK
                            txtUser.Text = "";
                            txtPassword.Text = "";

                            // *** CORRECT LOG CALL: Calls LogManager.Info (Yellow/Orange) ***
                            LogManager.Info($"Failed login attempt for username: {name}.");

                            UiHelper.ShowToast(this, "Invalid username or password.", "error");
                        }
                    }
                }
            }
        }
    }
}