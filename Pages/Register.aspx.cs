using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing; // Don't forget this for Color


namespace ASPWeBSM
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var logOutButton = Page.Master.FindControl("btnLogOut");
            logOutButton.Visible = false;
            UiHelper.ShowSessionToast(this);
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {

            // --- 1. NEW: Password Match Validation ---
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                lblMessage.Text = "Passwords do not match. Please enter the same password twice.";
                lblMessage.ForeColor = Color.Red;
                return; // Stop execution if passwords don't match
            }

            // Clear any previous error message before proceeding
            lblMessage.Text = string.Empty;


            string username = txtUser.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Text;
            bool isDup = false;

            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (var cmd = new SqlCommand("SELECT dbo.IsEmailDuplicate(@Email)", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    isDup = (bool)cmd.ExecuteScalar();
                }

                if (!isDup)
                {
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
                    LogManager.Success($"Registed successfully.");
                    UiHelper.SetToast("Registration successful. You can now log in.", "success");
                    // Optional but nice: redirect to login after successful registration
                    Response.Redirect("Login.aspx");
                }
                UiHelper.ShowToast(this, "Duplicate Email deteted", "error");
                txtUser.Text = "";
                txtEmail.Text = "";
                txtPassword.Text = "";
                txtConfirmPassword.Text = "";
            }
        }
    }
}