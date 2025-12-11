using System;
using System.Data;
using System.Data.SQLite; // CRITICAL: Use SQLite namespace
using System.Web; // CRITICAL: Needed to access HttpContext and Session

namespace ASPWeBSM
{
    public static class LogManager
    {
        // =============================================
        // PUBLIC FACADE METHODS (No change needed here)
        // =============================================

        public static void Info(string message) { Log(message, "INFO"); }
        public static void Success(string message) { Log(message, "SUCCESS"); }
        public static void Debug(string message) { /* ... config check and call Log ... */ }

        public static void Error(string message, Exception ex = null)
        {
            string fullMessage = message;
            if (ex != null)
            {
                fullMessage += " | EXCEPTION: " + ex.Message;
            }
            Log(fullMessage, "ERROR", ex);
        }

        // =============================================
        // CORE LOGGING LOGIC (SQLite Implementation)
        // =============================================

        private static void Log(string message, string level, Exception ex = null)
        {
            // 1. SAFELY RETRIEVE THE CURRENT USER'S ID FROM THE SESSION
            int? userId = null; // Use nullable int to handle cases where there is no user
            try
            {
                // HttpContext.Current gives access to the session in a static method
                if (System.Web.HttpContext.Current?.Session != null)
                {
                    if (int.TryParse(System.Web.HttpContext.Current.Session["UserId"]?.ToString(), out int id))
                    {
                        userId = id;
                    }
                }
            }
            catch { /* Suppress context errors */ }

            try
            {
                // CRITICAL: Use the dedicated SQLite connection
                using (var conn = DatabaseManager.GetLogConnection())
                {
                    conn.Open();

                    // CRITICAL FIX 1: Add UserId to the INSERT statement
                    string sql = "INSERT INTO Logs (LogLevel, Message, StackTrace, LogTime, UserId) VALUES (@Level, @Message, @StackTrace, CURRENT_TIMESTAMP, @UserId)";

                    using (var cmd = new SQLiteCommand(sql, conn)) // CRITICAL: Use SQLiteCommand
                    {
                        cmd.Parameters.AddWithValue("@Level", level);

                        // Prevent saving a massive message if it exceeds the column size
                        cmd.Parameters.AddWithValue("@Message", message.Substring(0, Math.Min(message.Length, 4000)));

                        // Use explicit if/else for maximum compatibility
                        object stackTraceParameter;
                        if (ex != null)
                        {
                            stackTraceParameter = ex.ToString();
                        }
                        else
                        {
                            stackTraceParameter = DBNull.Value;
                        }

                        cmd.Parameters.AddWithValue("@StackTrace", stackTraceParameter);

                        // CRITICAL FIX 2: Pass the retrieved UserId (or DBNull.Value if none)
                        cmd.Parameters.AddWithValue("@UserId", userId.HasValue ? (object)userId.Value : DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                // Suppress errors during logging
            }
        }
    }
}