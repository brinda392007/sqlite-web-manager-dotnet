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
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UiHelper.ShowSessionToast(this);
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("Users_CRUD", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EVENT", "INSERT");
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    // SP returns SCOPE_IDENTITY(), but we don't really need it here
                    object newId = cmd.ExecuteScalar();
                }
            }
            UiHelper.SetToast("Registration successful. You can now log in.", "success");
            // Optional but nice: redirect to login after successful registration
            Response.Redirect("Login.aspx");
        }
    }
}