using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Configuration;
using BCrypt.Net;

namespace ASPWeBSM
{
    // The class is made static as it only contains static members
    public static class DatabaseManager
    {
        private static string _connectionString;

        // Static constructor runs once when the class is first accessed
        static DatabaseManager()
        {
            // Fix: Use "AppDb" to match the Web.config entry.
            ConnectionStringSettings cs = WebConfigurationManager.ConnectionStrings["AppDb"];

            if (cs == null)
            {
                // Better error handling than a NullReferenceException
                throw new InvalidOperationException("Fatal Error: Connection string 'AppDb' not found in Web.config.");
            }

            _connectionString = cs.ConnectionString;
        }

        public static SqlConnection GetConnection()
        {
            // Now returns the correctly initialized connection string
            return new SqlConnection(_connectionString);
        }

        public static void Initialize()
        {
            // You should also confirm that you are calling DatabaseManager.Initialize() 
            // once when your application starts (e.g., in Global.asax Application_Start)
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
                // NOTE: It is a strong security practice to hash the newPassword 
                // before storing it in the database!
                // string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                using (var conn = GetConnection())
                {
                    conn.Open();
                    string sql = "UPDATE Users SET Password = @pass WHERE Email = @email";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@pass", newPassword);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // In a real application, log this exception (e.g., to a file or logging service)
                Console.WriteLine(ex);
            }
        }
    }
}