using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TestApplication1
{
    internal class Program
    {
        private static void Main()
        {
            Console.Write("Path for csv file: ");
            var dataTable = AnotherCsvLib.Parse.ReadFileToDataTable(Console.ReadLine());
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