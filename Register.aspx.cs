using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing; // Don't forget this for Color

namespace ASPWeBSM
{
    public partial class Register : System.Web.UI.Page
    {
        // ... (Page_Load remains the same)

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
            string password = txtPassword.Text; // Use the value from the first box

            using (var conn = DatabaseManager.GetConnection())
            {
                // ... (Rest of your database logic is here)
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

            // Optional but nice: show success message before redirect
            // Since we passed the password check, we can safely redirect
            Response.Redirect("Login.aspx");
        }
    }
}