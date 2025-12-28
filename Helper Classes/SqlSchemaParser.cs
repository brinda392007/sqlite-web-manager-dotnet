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
        [Serializable]//allows to store in ViewState
        public class SimpleColumn// simple column representation
        {
            public string Name { get; set; }//name of column
            public string DataType { get; set; }//datatype of the column
            public bool IsPrimaryKey { get; set; }//is primary key.. XD
        }

        //Main entry point function
        //accepts a filepath
        //Returns a Dictionary of String to list of simpleColumns
        public Dictionary<string, List<SimpleColumn>> ParseSqlFile(string filePath)
        {
            //schema declaration
            var schema = new Dictionary<string, List<SimpleColumn>>();

            // 1. Read the raw file
            string fileContent = File.ReadAllText(filePath);

            // 2. USE THE HELPER: Slice the file into individual CREATE TABLE statements
            //    This filters out INSERTs and keeps DacFx happy.

            // NOTE: The DacFx framework only accepts A single Batch of statements seprated by "GO" key words
            //       and the string extracted will be just a string with insert and select statements (unlikely but true)
            List<string> tableStatements = ExtractCreateTableStatements(fileContent);

            // 3. Initialize the Model
            // sqlServerVersion160 points to the SQL 2026 syntax,
            // TSqlModelOption gives the parameters that will spin up the model in memory
            using (var model = new TSqlModel(SqlServerVersion.Sql160, new TSqlModelOptions { }))
            {
                // 4. Feed the Function one batch at a time
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

                // 5. Extract schema from the Model into table format
                var tables = model.GetObjects(DacQueryScopes.UserDefined, Table.TypeClass);

                foreach (var table in tables)
                {
                    // Get tablename
                    string tableName = table.Name.Parts.Last();
                    var columnList = new List<SimpleColumn>();

                    // Get columns using your preferred relationship method
                    var columns = table.GetReferencedRelationshipInstances(Table.Columns);

                    foreach (var colObj in columns)
                    {
                        // initiate the column object
                        var col = colObj.Object;

                        // get datatype
                        var typeObj = col.GetReferenced(Column.DataType).FirstOrDefault();
                        string typeName = typeObj != null ? typeObj.Name.Parts.Last() : "nvarchar";

                        // Use existing helper for PKs
                        bool isPk = IsColumnPrimaryKey(table, col.Name.Parts.Last());

                        // Add to List of Columns
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
        // Instead of RegEx we use SqlParser160
        public List<string> ExtractCreateTableStatements(string rawSqlContent)
        {
            var createTableScripts = new List<string>();

            // 1. Initialize the parser (TSql160Parser supports modern SQL Server syntax)
            //    'true' enables QuotedIdentifier support.
            //    QuotedIdentifier support: accepts "Users" == Users, if false "Users" != Users
            TSqlParser parser = new TSql160Parser(true);

            using (var reader = new StringReader(rawSqlContent))
            {
                // 2. Parse the file into a syntax tree (known as Fragments)
                IList<ParseError> errors;
                TSqlFragment fragment = parser.Parse(reader, out errors);// inputs readed sql contents and outputs the errors into the <errors> var

                if (errors.Count > 0)
                {
                    // Pass errors into the log panel
                    foreach (var err in errors)
                    {
                        LogManager.Error(err.Message);

                    }

                    // 3. Use a ScriptGenerator to turn the syntax tree back into text strings later
                    SqlScriptGenerator generator = new Sql160ScriptGenerator();

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

                                    //adds string to the Table Scripts
                                    createTableScripts.Add(cleanSql);
                                }
                            }
                        }
                    }
                }

                return createTableScripts;
            }
        }

        // Finding a PK in Dac.Model is hard
        // because a PK is technically a "Constraint" that references a Table.
        // It's not just a property on the column,
        // So we use this approach.
        /*In the Microsoft SQL Model, a Column doesn't know it is a Primary Key.
            It works like a graph: [Table] <--- [Constraint] ---> [Columns]

            1. REVERSE LOOKUP (getPrimaryKeyConstraints):
                We stand on the Table and look BACKWARDS.
                "Who references ME as their Host?" 
                -> This finds the Constraint object.
                Code: table.GetReferencing(...)

            2. FORWARD LOOKUP (IsColumnPrimaryKey):
                We stand on the Constraint and look FORWARDS.
                "Who do YOU reference as your Columns?" 
                -> This finds the list of columns being controlled.
                Code: pk.GetReferenced(...)*/

        private bool IsColumnPrimaryKey(TSqlObject table, string columnName)
        {
            // get all the Primary key constraints
            var pkConstraints = getPrimaryKeyConstraints(table);

            foreach (var pk in pkConstraints)
            {
                //Get REFERENCED meaning the list of columns being referenced by this constraint
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
            //PrimaryKeyConstraint.Host gives the reference to the Table its been applied to 
            return table.GetReferencing(PrimaryKeyConstraint.Host, DacQueryScopes.UserDefined);
        }

    }
}