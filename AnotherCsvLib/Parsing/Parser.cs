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

        internal string ReadOneValueString()
        {
            return new string(ReadOneValueChars());
        }

        internal char[] ReadOneValueChars()
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
                        return chars.ToArray();

                    var nextCh = PeekChar();

                    if (ch == _options.QuoteChar)
                    {
                        if (nextCh == _options.QuoteChar)
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
                            return chars.ToArray();
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
                            return chars.ToArray();

                        if (ch == '\r' && nextCh == '\n' || ch == '\n')
                            return chars.ToArray();
                    }

                    chars.Add(ch.Value);
                }
                finally
                {
                    firstRead = false;
                }
            } while (true);
        }

        internal IEnumerable<string> ReadOneRowEnumerable()
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

                yield return ReadOneValueString();
            }
        }

        private IEnumerable<IEnumerable<string>> ReadAllRows()
        {
            while (PeekChar() != null)
            {
                yield return ReadOneRowEnumerable();
            }
        }

        internal string[][] ReadAsArrays()
        {
            return ReadAllRows().Select(x => x.ToArray()).ToArray();
        }

        internal DataTable ReadAsDataTable()
        {
            var reader = this;
            var dt = new DataTable();
            using (var rowEnumerator = reader.ReadAllRows().GetEnumerator())
            {
                if (!rowEnumerator.MoveNext() || rowEnumerator.Current == null)
                    throw new InvalidOperationException("There are no rows to read data from");

                foreach (var columnName in rowEnumerator.Current)
                {
                    dt.Columns.Add(columnName);
                }

                while (rowEnumerator.MoveNext())
                {
                    if (rowEnumerator.Current == null)
                        throw new InvalidOperationException("Row is null");
                    var dtRow = dt.NewRow();
                    var colIndex = 0;
                    foreach (var columnValue in rowEnumerator.Current)
                    {
                        dtRow[colIndex++] = columnValue;
                    }

                    dt.Rows.Add(dtRow);
                }
            }

            return dt;
        }
    }
}