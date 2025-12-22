using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASPWeBSM
{
    public partial class Analyze
    {
        private string GenerateSpScript(List<TableOperationSelection> selections, string dbPath)
        {
            var sb = new StringBuilder();

            foreach (var sel in selections)
            {
                var cols = GetTableColumns(dbPath, sel.TableName);
                if (cols.Count == 0) continue;

                var pk = cols.FirstOrDefault(c => c.IsPrimaryKey) ?? cols[0];
                var nonPkCols = cols.Where(c => c != pk).ToList();
                if (nonPkCols.Count == 0)
                    nonPkCols = cols; // fallback

                // header
                sb.AppendLine($"-- =============================================");
                sb.AppendLine($"-- Table: {sel.TableName}");
                sb.AppendLine($"-- Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"-- =============================================");
                sb.AppendLine($"IF OBJECT_ID('dbo.{sel.TableName}_CRUD','P') IS NOT NULL");
                sb.AppendLine($"    DROP PROC dbo.{sel.TableName}_CRUD;");
                sb.AppendLine("GO");
                sb.AppendLine();

                // procedure definition
                sb.AppendLine($"CREATE PROC dbo.{sel.TableName}_CRUD");
                sb.AppendLine("(");

                for (int i = 0; i < cols.Count; i++)
                {
                    var c = cols[i];
                    string comma = (i == cols.Count - 1) ? "" : ",";
                    sb.AppendLine($"    @{c.Name} {c.SqlType} = NULL{comma}");
                }

                sb.AppendLine($"    ,@EVENT VARCHAR(50) = NULL");
                sb.AppendLine(")");
                sb.AppendLine("AS");
                sb.AppendLine("BEGIN");
                sb.AppendLine("    SET NOCOUNT ON;");
                sb.AppendLine();

                bool firstBlock = true;

                if (sel.Select)
                {
                    sb.AppendLine("    IF (@EVENT = 'SELECT')");
                    sb.AppendLine("    BEGIN");
                    sb.AppendLine($"        SELECT * FROM {SqlName(sel.TableName)};");
                    sb.AppendLine("    END");
                    firstBlock = false;
                }

                if (sel.Insert)
                {
                    sb.AppendLine(firstBlock ? "    IF (@EVENT = 'INSERT')" : "    ELSE IF (@EVENT = 'INSERT')");
                    sb.AppendLine("    BEGIN");

                    string colList = string.Join(", ", nonPkCols.Select(c => SqlName(c.Name)));
                    string valList = string.Join(", ", nonPkCols.Select(c => "@" + c.Name));

                    sb.AppendLine($"        INSERT INTO {SqlName(sel.TableName)} ({colList})");
                    sb.AppendLine($"        VALUES ({valList});");
                    sb.AppendLine("    END");
                    firstBlock = false;
                }

                if (sel.Update)
                {
                    sb.AppendLine(firstBlock ? "    IF (@EVENT = 'UPDATE')" : "    ELSE IF (@EVENT = 'UPDATE')");
                    sb.AppendLine("    BEGIN");

                    string setList = string.Join(", ",
                        nonPkCols.Select(c => $"{SqlName(c.Name)} = @{c.Name}"));

                    sb.AppendLine($"        UPDATE {SqlName(sel.TableName)}");
                    sb.AppendLine($"        SET {setList}");
                    sb.AppendLine($"        WHERE {SqlName(pk.Name)} = @{pk.Name};");
                    sb.AppendLine("    END");
                    firstBlock = false;
                }

                if (sel.Delete)
                {
                    sb.AppendLine(firstBlock ? "    IF (@EVENT = 'DELETE')" : "    ELSE IF (@EVENT = 'DELETE')");
                    sb.AppendLine("    BEGIN");
                    sb.AppendLine($"        DELETE FROM {SqlName(sel.TableName)}");
                    sb.AppendLine($"        WHERE {SqlName(pk.Name)} = @{pk.Name};");
                    sb.AppendLine("    END");
                    firstBlock = false;
                }

                if (sel.SelectById)
                {
                    sb.AppendLine(firstBlock ? "    IF (@EVENT = 'SELECT_BY_ID')" : "    ELSE IF (@EVENT = 'SELECT_BY_ID')");
                    sb.AppendLine("    BEGIN");
                    sb.AppendLine($"        SELECT * FROM {SqlName(sel.TableName)}");
                    sb.AppendLine($"        WHERE {SqlName(pk.Name)} = @{pk.Name};");
                    sb.AppendLine("    END");
                }

                sb.AppendLine("END");
                sb.AppendLine("GO");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private string GenerateMethodsScript(List<TableOperationSelection> selections, string dbPath, bool parameterized)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Data.SqlClient;");
            sb.AppendLine("using System.Configuration;");
            sb.AppendLine();
            sb.AppendLine("public class GeneratedDbMethods");
            sb.AppendLine("{");
            sb.AppendLine("    private string _connString = ConfigurationManager.ConnectionStrings[\"AppDb\"].ConnectionString;");
            sb.AppendLine();

            foreach (var sel in selections)
            {
                var cols = GetTableColumns(dbPath, sel.TableName);
                if (cols.Count == 0) continue;

                var pk = cols.FirstOrDefault(c => c.IsPrimaryKey) ?? cols[0];
                var nonPkCols = cols.Where(c => c != pk).ToList();
                if (nonPkCols.Count == 0)
                    nonPkCols = cols;

                string procName = sel.TableName + "_CRUD";

                // SELECT
                if (sel.Select)
                {
                    sb.AppendLine($"    public DataTable {sel.TableName}_Select()");
                    sb.AppendLine("    {");
                    sb.AppendLine("        using (SqlConnection conn = new SqlConnection(_connString))");
                    sb.AppendLine($"        using (SqlCommand cmd = new SqlCommand(\"{procName}\", conn))");
                    sb.AppendLine("        {");
                    sb.AppendLine("            cmd.CommandType = CommandType.StoredProcedure;");
                    sb.AppendLine("            cmd.Parameters.AddWithValue(\"@EVENT\", \"SELECT\");");
                    sb.AppendLine("            using (SqlDataAdapter da = new SqlDataAdapter(cmd))");
                    sb.AppendLine("            {");
                    sb.AppendLine("                DataTable dt = new DataTable();");
                    sb.AppendLine("                da.Fill(dt);");
                    sb.AppendLine("                return dt;");
                    sb.AppendLine("            }");
                    sb.AppendLine("        }");
                    sb.AppendLine("    }");
                    sb.AppendLine();
                }

                // INSERT
                if (sel.Insert)
                {
                    if (parameterized)
                    {
                        string paramList = string.Join(", ", nonPkCols.Select(c => "string " + c.Name));
                        sb.AppendLine($"    public void {sel.TableName}_Insert({paramList})");
                    }
                    else
                    {
                        sb.AppendLine($"    public void {sel.TableName}_Insert()");
                    }

                    sb.AppendLine("    {");
                    sb.AppendLine("        using (SqlConnection conn = new SqlConnection(_connString))");
                    sb.AppendLine($"        using (SqlCommand cmd = new SqlCommand(\"{procName}\", conn))");
                    sb.AppendLine("        {");
                    sb.AppendLine("            cmd.CommandType = CommandType.StoredProcedure;");
                    sb.AppendLine("            cmd.Parameters.AddWithValue(\"@EVENT\", \"INSERT\");");

                    foreach (var c in nonPkCols)
                    {
                        if (parameterized)
                            sb.AppendLine($"            cmd.Parameters.AddWithValue(\"@{c.Name}\", {c.Name});");
                        else
                            sb.AppendLine($"            cmd.Parameters.AddWithValue(\"@{c.Name}\", /* TODO: assign value */);");
                    }

                    sb.AppendLine("            conn.Open();");
                    sb.AppendLine("            cmd.ExecuteNonQuery();");
                    sb.AppendLine("        }");
                    sb.AppendLine("    }");
                    sb.AppendLine();
                }

                // UPDATE
                if (sel.Update)
                {
                    if (parameterized)
                    {
                        string pkParam = "string " + pk.Name;
                        string otherParams = string.Join(", ", nonPkCols.Select(c => "string " + c.Name));

                        // Combine"string UserID, string Username, string Email..."
                        string fullParams = pkParam + (string.IsNullOrEmpty(otherParams) ? "" : ", " + otherParams);

                        sb.AppendLine($"    public void {sel.TableName}_Update({fullParams})");
                    }
                    else
                    {
                        sb.AppendLine($"    public void {sel.TableName}_Update()");
                    }

                    sb.AppendLine("    {");
                    sb.AppendLine("        using (SqlConnection conn = new SqlConnection(_connString))");
                    sb.AppendLine($"        using (SqlCommand cmd = new SqlCommand(\"{procName}\", conn))");
                    sb.AppendLine("        {");
                    sb.AppendLine("            cmd.CommandType = CommandType.StoredProcedure;");
                    sb.AppendLine("            cmd.Parameters.AddWithValue(\"@EVENT\", \"UPDATE\");");

                    foreach (var c in cols)
                    {
                        if (parameterized)
                            sb.AppendLine($"            cmd.Parameters.AddWithValue(\"@{c.Name}\", {c.Name});");
                        else
                            sb.AppendLine($"            cmd.Parameters.AddWithValue(\"@{c.Name}\", /* TODO: assign value */);");
                    }

                    sb.AppendLine("            conn.Open();");
                    sb.AppendLine("            cmd.ExecuteNonQuery();");
                    sb.AppendLine("        }");
                    sb.AppendLine("    }");
                    sb.AppendLine();
                }

                // DELETE
                if (sel.Delete)
                {
                    if (parameterized)
                    {
                        sb.AppendLine($"    public void {sel.TableName}_Delete(string {pk.Name})");
                    }
                    else
                    {
                        sb.AppendLine($"    public void {sel.TableName}_Delete()");
                    }

                    sb.AppendLine("    {");
                    sb.AppendLine("        using (SqlConnection conn = new SqlConnection(_connString))");
                    sb.AppendLine($"        using (SqlCommand cmd = new SqlCommand(\"{procName}\", conn))");
                    sb.AppendLine("        {");
                    sb.AppendLine("            cmd.CommandType = CommandType.StoredProcedure;");
                    sb.AppendLine("            cmd.Parameters.AddWithValue(\"@EVENT\", \"DELETE\");");

                    if (parameterized)
                        sb.AppendLine($"            cmd.Parameters.AddWithValue(\"@{pk.Name}\", {pk.Name});");
                    else
                        sb.AppendLine($"            cmd.Parameters.AddWithValue(\"@{pk.Name}\", /* TODO: assign value */);");

                    sb.AppendLine("            conn.Open();");
                    sb.AppendLine("            cmd.ExecuteNonQuery();");
                    sb.AppendLine("        }");
                    sb.AppendLine("    }");
                    sb.AppendLine();
                }

                // SELECT_BY_ID
                if (sel.SelectById)
                {
                    if (parameterized)
                    {
                        sb.AppendLine($"    public DataTable {sel.TableName}_SelectById(string {pk.Name})");
                    }
                    else
                    {
                        sb.AppendLine($"    public DataTable {sel.TableName}_SelectById()");
                    }

                    sb.AppendLine("    {");
                    sb.AppendLine("        using (SqlConnection conn = new SqlConnection(_connString))");
                    sb.AppendLine($"        using (SqlCommand cmd = new SqlCommand(\"{procName}\", conn))");
                    sb.AppendLine("        {");
                    sb.AppendLine("            cmd.CommandType = CommandType.StoredProcedure;");
                    sb.AppendLine("            cmd.Parameters.AddWithValue(\"@EVENT\", \"SELECT_BY_ID\");");

                    if (parameterized)
                        sb.AppendLine($"            cmd.Parameters.AddWithValue(\"@{pk.Name}\", {pk.Name});");
                    else
                        sb.AppendLine($"            cmd.Parameters.AddWithValue(\"@{pk.Name}\", /* TODO: assign value */);");

                    sb.AppendLine("            using (SqlDataAdapter da = new SqlDataAdapter(cmd))");
                    sb.AppendLine("            {");
                    sb.AppendLine("                DataTable dt = new DataTable();");
                    sb.AppendLine("                da.Fill(dt);");
                    sb.AppendLine("                return dt;");
                    sb.AppendLine("            }");
                    sb.AppendLine("        }");
                    sb.AppendLine("    }");
                    sb.AppendLine();
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
