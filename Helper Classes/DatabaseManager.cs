using System;
using System.Configuration;
using System.Data.SqlClient; // Keep for main tables (Users, Uploads)
using System.Data.SQLite;  // NEW: Add for Logs table
using System.IO;
using BCrypt.Net;

namespace ASPWeBSM
{
    public class DatabaseManager
    {
        // --- SQL SERVER Configuration (Main Application) ---
        private static string _sqlServerConnectionString =
            ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;

        // --- SQLITE Configuration (Logs Only) ---
        private static readonly string DbFileName = "Logs.sqlite";
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", DbFileName);
        private static readonly string _sqliteConnectionString = $"Data Source={DbPath};Version=3;";


        // SQL Server Connection (For Users, Uploads, etc.)
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_sqlServerConnectionString);
        }

        // SQLite Connection (For Logs only)
        public static SQLiteConnection GetLogConnection() // NEW METHOD
        {
            return new SQLiteConnection(_sqliteConnectionString);
        }


        public static void Initialize()
        {
            // 1. Ensure App_Data directory exists
            string appDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            // 2. Create SQLite file if it doesn't exist (Runs once)
            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
            }

            // --- SQL SERVER INITIALIZATION (Existing Logic) ---
            using (var conn = GetConnection()) // Uses SQL Server
            {
                conn.Open();

                // Create Users table if not exists (Keep as SQL Server syntax)
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
                { cmd.ExecuteNonQuery(); }

                // Create Uploads table if not exists (Keep as SQL Server syntax)
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
                { cmd.ExecuteNonQuery(); }

                // NOTE: We do NOT create the Logs table here anymore. It moves to SQLite.
            }

            // --- SQLITE INITIALIZATION (New Logic for Logs) ---
            using (var conn = GetLogConnection()) // Uses SQLite
            {
                conn.Open();

                string logsSql = @"
CREATE TABLE IF NOT EXISTS Logs -- SQLite syntax for creation
(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    LogLevel TEXT NOT NULL,
    Message TEXT NOT NULL,
    StackTrace TEXT NULL,
    LogTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UserId INTEGER NULL
    -- Note: We omit the FOREIGN KEY constraint here because the Users table is in SQL Server, not SQLite.
);";
                using (var cmd = new SQLiteCommand(logsSql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- resetPassword() method remains the same (uses SQL Server) ---
        internal static void resetPassword(string email, string newPassword)
        {
            // ... (Use GetConnection() for SQL Server password update) ...
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                using (var conn = GetConnection()) // Uses SQL Server
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
                Console.WriteLine(ex);
            }
        }
    }
}