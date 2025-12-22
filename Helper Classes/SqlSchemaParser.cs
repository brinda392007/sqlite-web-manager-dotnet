using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using Microsoft.SqlServer.Dac.Deployment;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace ASPWeBSM.Helper_Classes
{
    public class SqlSchemaParser
    {
        [Serializable]
        public class SimpleColumn
        {
            public string Name { get; set; }
            public string DataType { get; set; }
            public bool IsPrimaryKey { get; set; }
        }

        public Dictionary<string, List<SimpleColumn>> ParseSqlFile(string filePath)
        {
            var schema = new Dictionary<string, List<SimpleColumn>>();

            // 1. Read the raw file
            string fileContent = File.ReadAllText(filePath);

            // 2. USE THE HELPER: Slice the file into individual CREATE TABLE statements
            //    This filters out INSERTs and keeps DacFx happy.
            List<string> tableStatements = ExtractCreateTableStatements(fileContent);

            // 3. Initialize the Model
            using (var model = new TSqlModel(SqlServerVersion.Sql160, new TSqlModelOptions { }))
            {
                // 4. Feed the toaster one slice at a time
                foreach (var tableSql in tableStatements)
                {
                    try
                    {
                        model.AddObjects(tableSql);
                    }
                    catch (Exception ex)
                    {
                        // Log and skip bad tables so the whole app doesn't crash
                        System.Diagnostics.Debug.WriteLine($"Error parsing table chunk: {ex.Message}");
                    }
                }

                // 5. Extract schema (This part is mostly your original logic, just adapted)
                var tables = model.GetObjects(DacQueryScopes.UserDefined, Table.TypeClass);

                foreach (var table in tables)
                {
                    string tableName = table.Name.Parts.Last();
                    var columnList = new List<SimpleColumn>();

                    // Get columns using your preferred relationship method
                    var columns = table.GetReferencedRelationshipInstances(Table.Columns);

                    foreach (var colObj in columns)
                    {
                        var col = colObj.Object;

                        var typeObj = col.GetReferenced(Column.DataType).FirstOrDefault();
                        string typeName = typeObj != null ? typeObj.Name.Parts.Last() : "nvarchar";

                        // Use your existing helper for PKs
                        bool isPk = IsColumnPrimaryKey(table, col.Name.Parts.Last());

                        columnList.Add(new SimpleColumn
                        {
                            Name = col.Name.Parts.Last(),
                            DataType = typeName,
                            IsPrimaryKey = isPk
                        });
                    }

                    // Avoid duplicate keys if multiple chunks describe the same table (rare but possible)
                    if (!schema.ContainsKey(tableName))
                    {
                        schema.Add(tableName, columnList);
                    }
                }
            }

            return schema;
        }

        public List<string> ExtractCreateTableStatements(string rawSqlContent)
        {
            var createTableScripts = new List<string>();

            // 1. Initialize the parser (TSql150Parser supports modern SQL Server syntax)
            //    'true' enables QuotedIdentifier support.
            TSqlParser parser = new TSql150Parser(true);

            using (var reader = new StringReader(rawSqlContent))
            {
                // 2. Parse the file into a syntax tree (Fragment)
                IList<ParseError> errors;
                TSqlFragment fragment = parser.Parse(reader, out errors);

                if (errors.Count > 0)
                {
                    // Optional: Log parsing errors here if the uploaded SQL is invalid
                    foreach (var err in errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Parse Error: {err.Message} at line {err.Line}");
                    }
                    // Depending on your requirements, you might want to return empty or throw
                }

                // 3. Use a ScriptGenerator to turn the syntax tree back into text strings later
                SqlScriptGenerator generator = new Sql150ScriptGenerator();

                // 4. Traverse the batches and statements
                if (fragment is TSqlScript script)
                {
                    foreach (var batch in script.Batches)
                    {
                        foreach (var statement in batch.Statements)
                        {
                            // 5. FILTER: Only keep CREATE TABLE statements
                            if (statement is CreateTableStatement createTableStmt)
                            {
                                string cleanSql;
                                // Turn just this specific statement back into a string
                                generator.GenerateScript(createTableStmt, out cleanSql);

                                createTableScripts.Add(cleanSql);
                            }
                        }
                    }
                }
            }

            return createTableScripts;
        }

        private bool IsColumnPrimaryKey(TSqlObject table, string columnName)
        {
            var pkConstraints = getPrimaryKeyConstraints(table);

            foreach (var pk in pkConstraints)
            {
                //var pkColumns = pk.Object.GetReferenced(PrimaryKeyConstraint.Columns);
                var pkColumns = pk.GetReferenced(PrimaryKeyConstraint.Columns);
                if (pkColumns.Any(c => c.Name.Parts.Last() == columnName))
                {
                    return true;
                }
            }
            return false;
        }

        private static IEnumerable<TSqlObject> getPrimaryKeyConstraints(TSqlObject table)
        {
            return table.GetReferencing(PrimaryKeyConstraint.Host, DacQueryScopes.UserDefined);
        }

    }
}