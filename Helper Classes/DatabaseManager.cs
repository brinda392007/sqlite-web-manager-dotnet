using System;
using System.Configuration;
using System.Data.SqlClient;          // SQL Server (Main App)
using System.Data.SQLite;              // SQLite (Logs)
using System.IO;
using BCrypt.Net;
using System.Diagnostics;

namespace ASPWeBSM
{
    public class DatabaseManager
    {
        // =========================================
        // SQL SERVER CONFIG (MAIN APPLICATION)
        // =========================================
        private static readonly string _sqlServerConnectionString =
            ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;

        // =========================================
        // SQLITE CONFIG (LOGS ONLY)
        // =========================================
        private static readonly string LogDbFileName = "Logs_new.sqlite";
        private static readonly string AppDataPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
        private static readonly string LogDbPath =
            Path.Combine(AppDataPath, LogDbFileName);

        private static readonly string _sqliteConnectionString =
            $"Data Source={LogDbPath};Version=3;";

        // =========================================
        // CONNECTIONS
        // =========================================

        // SQL Server connection
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_sqlServerConnectionString);
        }

        // SQLite connection (Logs only)
        public static SQLiteConnection GetLogConnection()
        {
            return new SQLiteConnection(_sqliteConnectionString);
        }

        // =========================================
        // INITIALIZATION (RUNS ON APP START)
        // =========================================
        public static void Initialize()
        {
            // Ensure App_Data exists
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }

            // -----------------------------
            // SQL SERVER INIT (UNCHANGED)
            // -----------------------------
            using (var conn = GetConnection())
            {
                conn.Open();

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

            // -----------------------------
            // SQLITE LOG DB INIT (NEW)
            // -----------------------------
            using (var conn = GetLogConnection())
            {
                conn.Open(); // Auto-creates Logs.sqlite if missing

                string logsSql = @"
CREATE TABLE IF NOT EXISTS Logs
(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    LogType TEXT NOT NULL,      -- INFO / SUCCESS / ERROR
    Message TEXT NOT NULL,
    LogTime DATETIME DEFAULT CURRENT_TIMESTAMP
);";

                using (var cmd = new SQLiteCommand(logsSql, conn))
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
