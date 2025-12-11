using System;
using System.Configuration;
using System.Data.SqlClient; // Keep for main tables (Users, Uploads)
using System.Data.SQLite;  // NEW: Add for Logs table
using System.IO;
using BCrypt.Net;
using System.Diagnostics; // Added for debugging diagnostics

namespace ASPWeBSM
{
    public class DatabaseManager
    {
        // --- SQL SERVER Configuration (Main Application) ---
        private static string _sqlServerConnectionString =
            ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;

        // --- SQLITE Configuration (Logs Only) ---
        private static readonly string DbFileName = "Logs.sqlite";
        // Path.Combine is safer than concatenating strings
        private static readonly string AppDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
        private static readonly string DbPath = Path.Combine(AppDataPath, DbFileName);
        // Ensure the connection string includes the full path to the file
        private static readonly string _sqliteConnectionString = $"Data Source={DbPath};Version=3;";


        // SQL Server Connection (For Users, Uploads, etc.)
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_sqlServerConnectionString);
        }

        // SQLite Connection (For Logs only)
        public static SQLiteConnection GetLogConnection() // NEW METHOD
        {
            // The connection string uses the globally defined path
            return new SQLiteConnection(_sqliteConnectionString);
        }


        public static void Initialize()
        {
            // 1. Ensure App_Data directory exists
            if (!Directory.Exists(AppDataPath))
            {
                try
                {
                    Directory.CreateDirectory(AppDataPath);
                    Debug.WriteLine($"Created directory: {AppDataPath}");
                }
                catch (Exception ex)
                {
                    // This is a critical failure. Application cannot run without App_Data
                    Debug.WriteLine($"CRITICAL ERROR: Failed to create App_Data directory at {AppDataPath}. Check permissions. Error: {ex.Message}");
                    throw new InvalidOperationException($"CRITICAL DB ERROR: Failed to create log directory. Check file system permissions.", ex);
                }
            }

            // 2. Create SQLite file and table if it doesn't exist (Runs once)
            if (!File.Exists(DbPath))
            {
                try
                {
                    // Use the connection string to create the file
                    SQLiteConnection.CreateFile(DbPath);
                    Debug.WriteLine($"Created new SQLite database file: {DbPath}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"CRITICAL ERROR: Failed to create SQLite file at {DbPath}. Check permissions. Error: {ex.Message}");
                    throw new InvalidOperationException($"CRITICAL DB ERROR: Failed to create Logs.sqlite file. Check file system permissions.", ex);
                }
            }

            // --- SQL SERVER INITIALIZATION (Existing Logic) ---
            // (No change needed here, logic is sound)
            using (var conn = GetConnection()) // Uses SQL Server
            {
                conn.Open();
                // ... (Users and Uploads table creation logic remains here) ...

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
            }

            // --- SQLITE INITIALIZATION (Ensure Logs table has UserId) ---
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
    UserId INTEGER NULL -- CRITICAL: Ensure this column is present
);";
                using (var cmd = new SQLiteCommand(logsSql, conn))
                {
                    cmd.ExecuteNonQuery();
                    Debug.WriteLine("Ensured Logs table structure (including UserId) exists in SQLite.");
                }
            }
        }

        // --- resetPassword() method remains the same (uses SQL Server) ---
        internal static void resetPassword(string email, string newPassword)
        {
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
                Debug.WriteLine($"Error resetting password: {ex.Message}");
            }
        }
    }
}