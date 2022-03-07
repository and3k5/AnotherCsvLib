using System;
using System.IO;
using System.Linq;
using AnotherCsvLib.Writing.Reader;

namespace AnotherCsvLib.Writing
{
    internal class Writer : IDisposable
    {
        private readonly TableReader _reader;

        public Writer(TableReader reader)
        {
            _reader = reader;
        }

        public void Write(TextWriter writer, WriteOptions options)
        {
            {
                var firstColumn = true;
                foreach (var columnName in _reader.ReadColumns())
                {
                    if (!firstColumn)
                        writer.Write(options.ColumnSeparator);
                    writer.Write(EncodeToCell(columnName, options));
                    firstColumn = false;
                }
            }

            foreach (var row in _reader.ReadRows())
            {
                writer.WriteLine();
                var firstColumn = true;
                foreach (var cell in row.ReadCells())
                {
                    if (!firstColumn)
                        writer.Write(options.ColumnSeparator);
                    writer.Write(EncodeToCell(cell.ReadValue().ToString(), options));
                    firstColumn = false;
                }
            }
        }

        private static string EncodeToCell(string value, CsvOptionsBase options)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            if (value.Contains("\n") || value.Contains(options.ColumnSeparator) || value.Contains(options.QuoteChar))
            {
                value = options.QuoteChar +
                        value.Replace(options.QuoteChar.ToString(), $"{options.QuoteChar}{options.QuoteChar}") +
                        options.QuoteChar;
            }

            return value;
        }

        public void Dispose()
        {
            _reader?.Dispose();
        }
    }
}