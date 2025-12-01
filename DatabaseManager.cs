using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Web;

namespace ASPWeBSM
{
    public class DatabaseManager
    {
        //Connection String 
        //We use "|DataDirectory|" so it works on your laptop and on the server without changing code 
        private static string _connectionString = "Data Source = |DataDirectory|\\ASPWeBSM.db; Version=3;";
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
        
        public static void Initialize()
        {
            string dbPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/ASPWeBSM.db");

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"CREATE TABLE IF NOT EXISTS Users (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Username TEXT NOT NULL,
                                    Email TEXT NOT NULL UNIQUE,
                                    Password TEXT NOT NULL,
                                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                                );";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                string uploadSql = @"CREATE TABLE IF NOT EXISTS Uploads(
                                        	Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        	UserId INTEGER NOT NULL,
                                        	FileName TEXT NOT NULL,
                                        	ContentType TEXT NOT NULL,
                                        	Content BLOB NOT NULL,
                                        	Size INTEGER NOT NULL,
                                        	UploadedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                                        	FOREIGN KEY (UserId) REFERENCES Users(Id)
                                        );";

                using (var command = new SQLiteCommand(uploadSql, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}