using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AnotherCsvLib;

namespace TestApplication1
{
    internal class Program
    {
        private static void Main()
        {
            askForDelimiter:
            Console.Write("Delimiter (;): ");
            var delimiterStr = Console.ReadLine();
            if (string.IsNullOrEmpty(delimiterStr))
            {
                delimiterStr = ";";
            }
            if (delimiterStr.Length > 1)
            {
                Console.WriteLine("Delimiter cannot be more than one character!");
                goto askForDelimiter;
            }
            Console.Write("Path for csv file: ");
            var dataTable = AnotherCsvLib.Parse.ReadFileToDataTable(Console.ReadLine(), new ParseOptions()
            {
                ColumnSeparator = delimiterStr[0],
            });
            var data = new Dictionary<string, List<string>>();

            var maxLengths = new Dictionary<string, int>();

            foreach (DataColumn col in dataTable.Columns)
            {
                var values = new List<string>();
                data.Add(col.ColumnName, values);
                values.AddRange(from DataRow row in dataTable.Rows select row[col]?.ToString());

                maxLengths.Add(col.ColumnName, values.Concat(new[] {col.ColumnName}).Max(x => x?.Length ?? 0));
            }


            const int margin = 2;

            foreach (var value in data.Keys)
            {
                Console.Write(value);
                Console.Write(new string(' ', maxLengths[value] + margin - value.Length));
            }

            Console.WriteLine();

            var rowsCount = data.Values.GroupBy(x => x.Count).Single().Key;


            for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++)
            {
                foreach (var key in data.Keys)
                {
                    var value = data[key][rowIndex];
                    Console.Write(value);
                    Console.Write(new string(' ', maxLengths[key] + margin - value.Length));
                }

                Console.WriteLine();
            }
        }
    }
}