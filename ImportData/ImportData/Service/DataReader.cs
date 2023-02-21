using ImportData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportData.Service
{
    public class DataReader
    {
        private List<ImportedObject> ImportedObjects;

        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            ImportedObjects = new List<ImportedObject>();

            using (var streamReader = new StreamReader(fileToImport))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var values = line.Split(';');

                    if (values.Length == 7)
                    {
                        var importedObject = new ImportedObject
                        {
                            Type = values[0].Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper(),
                            Name = values[1].Trim().Replace(" ", "").Replace(Environment.NewLine, ""),
                            Schema = values[2].Trim().Replace(" ", "").Replace(Environment.NewLine, ""),
                            ParentName = values[3].Trim().Replace(" ", "").Replace(Environment.NewLine, ""),
                            ParentType = values[4].Trim().Replace(" ", "").Replace(Environment.NewLine, ""),
                            DataType = values[5].Trim(),
                            IsNullable = values[6].Trim(),
                        };

                        ImportedObjects.Add(importedObject);
                    }
                }
            }

            // Assign number of children
            foreach (var importedObject in ImportedObjects)
            {
                foreach (var impObj in ImportedObjects)
                {
                    if (impObj.ParentType == importedObject.Type && impObj.ParentName == importedObject.Name)
                    {
                        importedObject.NumberOfChildren++;
                    }
                }
            }

            // Print data
            if (printData)
            {
                foreach (var database in ImportedObjects.Where(x => x.Type == "DATABASE"))
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // Print all database's tables
                    foreach (var table in ImportedObjects.Where(x => x.ParentType.ToUpper() == database.Type && x.ParentName == database.Name))
                    {
                        Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                        // Print all table's columns
                        foreach (var column in ImportedObjects.Where(x => x.ParentType.ToUpper() == table.Type && x.ParentName == table.Name))
                        {
                            Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
