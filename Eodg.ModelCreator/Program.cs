using Dapper;
using Eodg.ModelCreator.Models;
using Fclp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Eodg.ModelCreator
{
    class Program
    {
        private static string _connectionString;
        private static string _outputDirectory;
        private static string _namespace;
        private static DataConnection _dataConnection;

        static void Main(string[] args)
        {
            HandleArguments(args);

            _dataConnection = new DataConnection(_connectionString);

            _dataConnection
                .RunQuery<Table>("SELECT name FROM sys.tables")
                .AsList()
                .ForEach(table => ProcessTable(table));

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static IEnumerable<Property> GetProperties(string tableName)
        {
            var propertyScript = File.ReadAllText($"{Directory.GetCurrentDirectory()}/property-script.sql").Replace("@TableName", tableName);

            return _dataConnection.RunQuery<Property>(propertyScript);
        }

        private static void GenerateFile(string tableName, IEnumerable<Property> properties)
        {
            string fileContents = string.Empty;

            var usings = GetUsings(properties);

            if (usings.Count() > 0)
            {
                fileContents += String.Join("\n", usings);

                fileContents += "\n\n";
            }

            fileContents += $"namespace {_namespace}\n{{\n    public class {tableName}\n    {{\n        ";

            fileContents += String.Join("\n\n        ", properties.Select(p => p.PropertyDefinition));

            fileContents += "\n    }\n}";

            var path = Path.Combine(_outputDirectory, $"{tableName}.cs");

            File.WriteAllText(path, fileContents);

            Console.WriteLine($"{path} written.");
        }

        private static bool HasNullPropertyDefinitions(IEnumerable<Property> properties)
        {
            var nullProperties = properties.Where(p => String.IsNullOrEmpty(p.PropertyDefinition));

            if (nullProperties.Any())
            {
                Console.WriteLine("ERROR:");

                nullProperties.AsList().ForEach(n =>
                {
                    Console.WriteLine($"Datatype `{n.DataType}` not accounted for in SQL script.");
                });

                return true;
            }

            return false;
        }

        private static void HandleArguments(string[] args)
        {
            var parser = new FluentCommandLineParser();

            parser.Setup<string>('c', "connection-string")
                .Callback(connection => _connectionString = connection)
                .Required();

            parser.Setup<string>('o', "output-directory")
             .Callback(outputDirectory => _outputDirectory = outputDirectory)
             .Required();

            parser.Setup<string>('n')
             .Callback(classNamespace => _namespace = classNamespace)
             .Required();

            var result = parser.Parse(args);

            if (result.HasErrors)
            {
                Console.WriteLine("Invalid syntax.");
                Console.WriteLine("Proper usage: `dotnet Eodg.ModelCreator.dll -c <connection string> -o <output directory> -n <namespace>");
                Environment.Exit(0);
            }
        }

        private static IEnumerable<string> GetUsings(IEnumerable<Property> properties)
        {
            var usings = new List<string>();

            var needsSystemUsings = properties.Where(p => p.DataType == "date" || p.DataType == "datetime").Count() > 0;

            if (needsSystemUsings)
            {
                usings.Add("using System;");
            }

            // May need more cases... not sure...

            return usings;
        }

        private static void ProcessTable(Table table)
        {
            var properties = GetProperties(table.Name);

            if (HasNullPropertyDefinitions(properties))
            {
                Console.WriteLine($"No class written for table `{table}`");
                return;
            }

            GenerateFile(table.Name, properties);
        }
    }
}
