using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AnotherCsvLib.Parsing
{
    internal class Parser : IDisposable
    {
        private readonly TextReader _reader;
        private readonly ParseOptions _options;

        public Parser(TextReader reader, ParseOptions options)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Dispose()
        {
            _reader?.Dispose();
        }

        private static char? ToChar(int v)
        {
            if (v == -1)
                return null;
            return (char) v;
        }

        private char? PeekChar() => ToChar(_reader.Peek());

        private char? ReadChar() => ToChar(_reader.Read());

        private void SkipChar() => _reader.Read();

        internal object ReadOneValueObject()
        {
            var firstRead = true;
            var insideQuotedValue = false;

            var chars = new List<char>();

            do
            {
                try
                {
                    var ch = ReadChar();

                    if (ch == null)
                        break;

                    var nextCh = PeekChar();

                    if (ch == _options.QuoteChar)
                    {
                        if (!firstRead && nextCh == _options.QuoteChar)
                        {
                            chars.Add(_options.QuoteChar);
                            SkipChar();
                            continue;
                        }

                        if (!firstRead)
                        {
                            if (!insideQuotedValue)
                                throw new InvalidOperationException("Invalid CSV content");

                            SkipChar();
                            break;
                        }
                        else
                        {
                            insideQuotedValue = true;
                            continue;
                        }
                    }

                    if (!insideQuotedValue)
                    {
                        if (ch == _options.ColumnSeparator)
                            break;

                        if (ch == '\r' && nextCh == '\n' || ch == '\n')
                            break;
                    }

                    chars.Add(ch.Value);
                }
                finally
                {
                    firstRead = false;
                }
            } while (true);

            if (!insideQuotedValue && chars.Count == 0)
                return null;

            return new string(chars.ToArray());
        }

        internal IEnumerable<object> ReadOneRowEnumerable()
        {
            while (true)
            {
                if (PeekChar() == '\r')
                    SkipChar();
                if (PeekChar() == null)
                    break;
                if (PeekChar() == '\n')
                {
                    SkipChar();
                    break;
                }

                yield return ReadOneValueObject();
            }
        }

        private IEnumerable<IEnumerable<object>> ReadAllRows()
        {
            while (PeekChar() != null)
            {
                yield return ReadOneRowEnumerable();
            }
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