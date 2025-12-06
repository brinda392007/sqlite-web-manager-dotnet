using System;
using System.Configuration;
using System.Data.SqlClient;
using BCrypt.Net;

namespace ASPWeBSM
{
    public class DatabaseManager
    {
        // Connection string from Web.config
        private static string _connectionString =
            ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public static void Initialize()
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // Create Users table if not exists
                string usersSql = @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE dbo.Users
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        [Password] NVARCHAR(255) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT(GETDATE())
    );
END";

                using (var cmd = new SqlCommand(usersSql, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create Uploads table if not exists
                string uploadsSql = @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Uploads')
BEGIN
    CREATE TABLE dbo.Uploads
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        FileName NVARCHAR(255) NOT NULL,
        ContentType NVARCHAR(100) NOT NULL,
        Content VARBINARY(MAX) NOT NULL,
        Size BIGINT NOT NULL,
        UploadedAt DATETIME NOT NULL DEFAULT(GETDATE()),
        CONSTRAINT FK_Uploads_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
END";

                using (var cmd = new SqlCommand(uploadsSql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        internal static void resetPassword(string email, string newPassword)
        {
            try
            {
                using(var conn = GetConnection())
                {
                    conn.Open();
                    string sql = "UPDATE Users SET Password = @pass WHERE Email = @email";
                    using(var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@pass", newPassword);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
