using System;
using System.Data;
using System.Data.SQLite; // CRITICAL: Use SQLite namespace

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
            try
            {
                // CRITICAL: Use the dedicated SQLite connection
                using (var conn = DatabaseManager.GetLogConnection())
                {
                    conn.Open();

                    // SQLite SQL query
                    string sql = "INSERT INTO Logs (LogLevel, Message, StackTrace, LogTime) VALUES (@Level, @Message, @StackTrace, CURRENT_TIMESTAMP)";

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

                        // NOTE: Since the Logs table in SQLite is now decoupled from Users, 
                        // we cannot easily save the UserId here without another lookup step, 
                        // which we will skip for now to simplify the LogManager.

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