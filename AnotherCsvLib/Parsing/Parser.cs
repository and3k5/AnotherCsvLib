using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AnotherCsvLib.Parsing.Reader;

namespace AnotherCsvLib.Parsing
{
    internal class Parser : IDisposable
    {
        private readonly CharReader _reader;
        private readonly RowReader _rowReader;

        public Parser(CharReader reader, ParseOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));

            _rowReader = new RowReader(new ValueReader(_reader, options));
        }

        public void Dispose()
        {
            _reader?.Dispose();
        }

        private IEnumerable<object[]> ReadAllRows()
        {
            var rows = new List<object[]>();
            while (_reader.PeekChar() != null)
            {
                rows.Add(_rowReader.ReadOneRowEnumerable());
            }

            return rows.ToArray();
        }

        internal string[][] ReadAsArrays()
        {
            return ReadAllRows().Select(x => x.Select(MakeString).ToArray()).ToArray();
        }

        private static string MakeString(object o)
        {
            if (o is string str)
                return str;
            return o?.ToString();
        }

        internal DataTable ReadAsDataTable()
        {
            var reader = this;
            var dt = new DataTable();
            using (var rowEnumerator = reader.ReadAllRows().GetEnumerator())
            {
                if (!rowEnumerator.MoveNext() || rowEnumerator.Current == null)
                    throw new InvalidOperationException("There are no rows to read data from");

                var skippedColumns = new List<int>();

                var colIndex = 0;
                foreach (var col in rowEnumerator.Current)
                {
                    if (col is string columnName && !string.IsNullOrEmpty(columnName))
                    {
                        dt.Columns.Add(columnName);
                    }
                    else
                    {
                        skippedColumns.Add(colIndex);
                    }

                    colIndex++;
                }

                while (rowEnumerator.MoveNext())
                {
                    if (rowEnumerator.Current == null)
                        throw new InvalidOperationException("Row is null");
                    var dtRow = dt.NewRow();
                    colIndex = 0;
                    var actualColIndex = 0;
                    foreach (var columnValue in rowEnumerator.Current)
                    {
                        if (!skippedColumns.Contains(actualColIndex) && dt.Columns.Count > colIndex)
                        {
                            dtRow[colIndex] = columnValue;
                            colIndex++;
                        }

                        actualColIndex++;
                    }

                    dt.Rows.Add(dtRow);
                }
            }

            return dt;
        }
    }
}