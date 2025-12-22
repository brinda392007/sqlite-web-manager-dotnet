using System;
using System.Data.SqlClient; // 🔁 CHANGED: SQLite → SQL Server
using System.Web;

namespace ASPWeBSM
{
    public static class LogManager
    {
        // ==========================
        // PUBLIC METHODS (DO NOT CHANGE)
        // ==========================

        public static void Info(string message)
        {
            WriteLog("INFO", message);
        }

        public static void Success(string message)
        {
            WriteLog("SUCCESS", message);
        }

        public static void Error(string message)
        {
            WriteLog("ERROR", message);
        }

        // ==========================
        // CORE LOGIC (SQL SERVER)
        // ==========================

        private static void WriteLog(string logType, string message)
        {
            int? userId = GetCurrentUserId();
            if (userId == null) return; // No user = no log (same behavior)

            try
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();

                    // 1️⃣ INSERT LOG (SQL Server)
                    string insertSql = @"
INSERT INTO Logs (UserId, LogType, Message)
VALUES (@UserId, @LogType, @Message);";

                    using (var cmd = new SqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId.Value);
                        cmd.Parameters.AddWithValue("@LogType", logType);
                        cmd.Parameters.AddWithValue("@Message", message);

                        cmd.ExecuteNonQuery();
                    }

                    // 2️⃣ KEEP ONLY LAST 100 LOGS PER USER
                    string cleanupSql = @"
DELETE FROM Logs
WHERE LogID NOT IN
(
    SELECT TOP 100 LogID
    FROM Logs
    WHERE UserId = @UserId
    ORDER BY LogTime DESC
)
AND UserId = @UserId;";

                    using (var cleanupCmd = new SqlCommand(cleanupSql, conn))
                    {
                        cleanupCmd.Parameters.AddWithValue("@UserId", userId.Value);
                        cleanupCmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // ❗ NEVER allow logging to crash the app
               
            }
        }

        // ==========================
        // SESSION HELPER
        // ==========================

        private static int? GetCurrentUserId()
        {
            try
            {
                if (HttpContext.Current?.Session?["UserId"] != null)
                {
                    if (int.TryParse(
                        HttpContext.Current.Session["UserId"].ToString(),
                        out int id))
                    {
                        return id;
                    }
                }
            }
            catch { }

            return null;
        }
    }
}
