using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASPWeBSM
{
    public partial class Register : System.Web.UI.Page
    {
        private static string _connectionString = "Data Source=|DataDirectory|//ASPWeBSM.db; Version=3;";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            using(var conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                string sql = "INSERT INTO Users (Username, Email, Password) VALUES (@Name, @Email, @Pass)";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", username);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Pass", password);

                    cmd.ExecuteNonQuery();
                }

            }
        }
    }
}