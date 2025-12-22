using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using Microsoft.SqlServer.Dac.Deployment;
using Microsoft.SqlServer.Dac.Model;

namespace ASPWeBSM.Helper_Classes
{
    public class SqlSchemaParser
    {
        public class SimpleColumn
        {
            public string Name { get; set; }
            public string DataType { get; set; }
            public bool IsPrimaryKey { get; set; }
        }

        public Dictionary<string, List<SimpleColumn>> ParseSqlFile(string filePath)
        {
            var schema = new Dictionary<string, List<SimpleColumn>>();
            string fileContent = File.ReadAllText(filePath);

            using (var model = new TSqlModel(SqlServerVersion.Sql160, new TSqlModelOptions { }))
            {
                model.AddObjects(fileContent);

                var tables = model.GetObjects(DacQueryScopes.UserDefined, Table.TypeClass);

                foreach (var table in tables)
                {
                    string tableName = table.Name.Parts.Last();
                    var columnList = new List<SimpleColumn>();

                    var columns = table.GetReferencedRelationshipInstances(Table.Columns);

                    foreach (var colObj in columns)
                    {
                        var col = colObj.Object;

                        var typeObj = col.GetReferenced(Column.DataType).FirstOrDefault();
                        string typeName = typeObj != null ? typeObj.Name.Parts.Last() : "nvarchar";

                        bool isPk = IsColumnPrimaryKey(table, col.Name.Parts.Last());

                        columnList.Add(new SimpleColumn
                        {
                            Name = col.Name.Parts.Last(),
                            DataType = typeName,
                            IsPrimaryKey = isPk
                        });
                    }

                    schema.Add(tableName, columnList);

                }
            }

            return schema;
        }

        private bool IsColumnPrimaryKey(TSqlObject table, string columnName)
        {
            var pkConstraints = getPrimaryKeyConstraints(table);

            foreach (var pk in pkConstraints)
            {
                //var pkColumns = pk.Object.GetReferenced(PrimaryKeyConstraint.Columns);
                var pkColumns = pk.GetReferenced(PrimaryKeyConstraint.Columns);
                if(pkColumns.Any(c => c.Name.Parts.Last() == columnName))
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