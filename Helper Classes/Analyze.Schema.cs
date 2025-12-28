using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ASPWeBSM
{
    public partial class Analyze
    {
        private List<ColumnInfo> GetTableColumns(string dbPath, string tableName)
        {
            // If file is .sql we do not need to validate it 
            if (CurrentSqlSchema != null && CurrentSqlSchema.ContainsKey(tableName))
            {
                var sqlCols = CurrentSqlSchema[tableName];
                var result = new List<ColumnInfo>();

                foreach (var col in sqlCols)
                {
                    result.Add(new ColumnInfo
                    {
                        Name = col.Name,
                        SqlType = col.DataType, 
                        IsPrimaryKey = col.IsPrimaryKey
                    });
                }
                return result;
            }
            // If it is a .db file add that too the validator
            else
            {
                var cols = new List<ColumnInfo>();
                string connStr = $"Data Source={dbPath};Version=3;";
                using (var conn = new SQLiteConnection(connStr))
                {
                    conn.Open();

                    // Read the Pragma Table and get all the Data
                    string sql = $"PRAGMA table_info([{tableName}]);";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        // Read each data till there is data
                        while (reader.Read())
                        {
                            string colName = reader["name"].ToString();
                            string type = reader["type"].ToString();
                            bool isPk = Convert.ToInt32(reader["pk"]) == 1;

                            cols.Add(new ColumnInfo
                            {
                                Name = colName,
                                SqlType = MapSqlType(type),
                                IsPrimaryKey = isPk
                            });
                        }
                    }
                }
                return cols;
            }

        }

        // sqlite is Typless but Sql is not
        private string MapSqlType(string sqliteType)
        {
            string t = (sqliteType ?? "").ToUpperInvariant();

            if (t.Contains("INT"))
                return "INT";

            if (t.Contains("CHAR") || t.Contains("CLOB") || t.Contains("TEXT"))
                return "NVARCHAR(255)";

            if (t.Contains("REAL") || t.Contains("FLOA") || t.Contains("DOUB"))
                return "FLOAT";

            if (t.Contains("BLOB"))
                return "VARBINARY(MAX)";

            // default
            return "NVARCHAR(255)";
        }

        // sometimes some Identifier names are already taken by System.
        // So to mark that its User Defined square braces("[ ]") are added 
        private string SqlName(string name)
        {
            return "[" + name + "]";
        }
    }
}
