using System;
using System.Collections.Generic;
using System.Data.SQLite;
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

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                string name = txtUser.Text;
                string pass = txtPassword.Text;

                string sql = "SELECT * FROM Users WHERE Username = @Name AND Password = @Pass";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Pass", pass);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Session["UserId"] = reader["Id"];
                            Session["Username"] = reader["Username"];
                            Response.Redirect("Default.aspx");
                        }
                        else
                        {
                            txtUser.Text = "";
                            txtPassword.Text = "";
                            btnLogin.Text = "Invalid, Try again";
                        }
                    }
                }
            }
        }
    }
}