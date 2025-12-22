using System;
using System.Configuration;
using System.Data.SqlClient;   // SQL Server (Main App + Logs)
using System.IO;
using BCrypt.Net;
using System.Diagnostics;

namespace ASPWeBSM
{
    public class DatabaseManager
    {
        // =========================================
        // SQL SERVER CONFIG (MAIN APPLICATION + LOGS)
        // =========================================
        private static readonly string _sqlServerConnectionString =
            ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;

        // =========================================
        // CONNECTION
        // =========================================
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_sqlServerConnectionString);
        }

        // =========================================
        // INITIALIZATION (RUNS ON APP START)
        // =========================================
        public static void Initialize()
        {
            // Ensure App_Data exists (used elsewhere in app)
            string appDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            using (var conn = GetConnection())
            {
                conn.Open();

                // -----------------------------
                // USERS TABLE (UNCHANGED)
                // -----------------------------
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

                // -----------------------------
                // UPLOADS TABLE (UNCHANGED)
                // -----------------------------
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

                // -----------------------------
                // LOGS TABLE (NEW → SQL SERVER)
                // -----------------------------
                string logsSql = @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Logs')
BEGIN
    CREATE TABLE dbo.Logs
    (
        LogID INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        LogType VARCHAR(20) NOT NULL,      -- INFO / SUCCESS / ERROR
        Message NVARCHAR(MAX) NOT NULL,
        LogTime DATETIME NOT NULL DEFAULT(GETDATE()),

        CONSTRAINT FK_Logs_Users FOREIGN KEY (UserId)
        REFERENCES dbo.Users(Id)
    );
END";
                using (var cmd = new SqlCommand(logsSql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // =========================================
        // PASSWORD RESET (UNCHANGED)
        // =========================================
        internal static void resetPassword(string email, string newPassword)
        {
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                using (var conn = GetConnection())
                {
                    conn.Open();

                    string sql = "UPDATE Users SET Password = @pass WHERE Email = @email";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@pass", hashedPassword);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error resetting password: {ex.Message}");
            }
        }
    }
}
