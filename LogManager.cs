using System;
using System.Data.SQLite;
using System.Web;

namespace ASPWeBSM
{
    public static class LogManager
    {
        // ==========================
        // PUBLIC METHODS (USE THESE)
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
        // CORE LOGIC (PRIVATE)
        // ==========================

        private static void WriteLog(string logType, string message)
        {
            int? userId = GetCurrentUserId();
            if (userId == null) return; // No user = no log

            try
            {
                using (var conn = DatabaseManager.GetLogConnection())
                {
                    conn.Open();

                    // 1️⃣ Insert log
                    string insertSql = @"
INSERT INTO Logs (UserId, LogType, Message)
VALUES (@UserId, @LogType, @Message);";

                    using (var cmd = new SQLiteCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@LogType", logType);
                        cmd.Parameters.AddWithValue("@Message", message);

                        cmd.ExecuteNonQuery();
                    }

                    // 2️⃣ Keep only latest 100 logs per user
                    string cleanupSql = @"
DELETE FROM Logs
WHERE Id NOT IN
(
    SELECT Id FROM Logs
    WHERE UserId = @UserId
    ORDER BY LogTime DESC
    LIMIT 100
)
AND UserId = @UserId;";

                    using (var cleanupCmd = new SQLiteCommand(cleanupSql, conn))
                    {
                        cleanupCmd.Parameters.AddWithValue("@UserId", userId.Value);
                        cleanupCmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // NEVER break app because of logging
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
