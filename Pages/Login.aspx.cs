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
            var logOutButton = Page.Master.FindControl("btnLogOut");
            logOutButton.Visible = false;
            UiHelper.ShowSessionToast(this);
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string name = txtUser.Text;
            string pass = txtPassword.Text;

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
                            Session["UserId"] = reader["Id"];
                            Session["Username"] = reader["Username"];
                            UiHelper.SetToast("Login successful", "success");
                            Response.Redirect("Default.aspx");
                        }
                        else
                        {
                            txtUser.Text = "";
                            txtPassword.Text = "";
                            UiHelper.ShowToast(this, "Invalid username or password.", "error");
                        }
                    }
                }
            }
        }
    }
}