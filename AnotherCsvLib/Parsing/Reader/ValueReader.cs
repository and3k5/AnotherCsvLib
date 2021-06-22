using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AnotherCsvLib.Parsing.Reader
{
    internal class ValueReader
    {
        private readonly CharReader _reader;
        private readonly ParseOptions _options;

        public ValueReader(CharReader reader, ParseOptions options)
        {
            _reader = reader;
            _options = options;
        }

        internal (bool endRow, object value) ReadOneValueObject(int rowIndex, int colIndex)
        {
            var insideQuotedValue = false;

            var chars = new List<char>();
            var endRow = false;
            var nextI = 0;

            do
            {
                var i = nextI++;

                if (!insideQuotedValue && PeekAndSkipNewline())
                {
                    endRow = true;
                    break;
                }

                var ch = _reader.ReadChar();

                if (ch == null)
                {
                    endRow = true;
                    break;
                }

                var nextCh = _reader.PeekChar();

                if (ch == _options.QuoteChar)
                {
                    if (i <= 0)
                    {
                        insideQuotedValue = true;
                        continue;
                    }

                    if (nextCh == _options.QuoteChar)
                    {
                        if (!insideQuotedValue)
                            throw new Exception(
                                $"Invalid CSV content at line {rowIndex + 1} and column {colIndex + 1}");

                        chars.Add(_options.QuoteChar);
                        _reader.SkipChar();
                        continue;
                    }

                    if (nextCh == null)
                    {
                        endRow = true;
                        break;
                    }

                    if (nextCh == _options.ColumnSeparator)
                    {
                        if (!insideQuotedValue)
                            chars.Add(ch.Value);

                        _reader.SkipChar();
                        break;
                    }

                    if (PeekAndSkipNewline())
                    {
                        if (!insideQuotedValue)
                            chars.Add(ch.Value);
                        endRow = true;
                        break;
                    }
                }

                if (!insideQuotedValue)
                {
                    if (ch == _options.ColumnSeparator)
                    {
                        //_reader.SkipChar();
                        break;
                    }

                    if (nextCh == _options.ColumnSeparator)
                    {
                        chars.Add(ch.Value);
                        _reader.SkipChar();
                        break;
                    }

                    if (PeekAndSkipNewline())
                    {
                        chars.Add(ch.Value);
                        endRow = true;
                        break;
                    }
                }

                chars.Add(ch.Value);
            } while (true);


            string value = null;
            if (insideQuotedValue || chars.Count != 0)
            {
                value = new string(chars.ToArray());
            }

            return (endRow, value);
        }

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        private bool PeekAndSkipNewline()
        {
            bool IsMatch(string test)
            {
                var match = string.Equals(test, _reader.PeekString(test.Length), StringComparison.Ordinal);
                if (match)
                    _reader.SkipChars(test.Length);
                return match;
            }

            if (!_reader.PeekChar().HasValue)
                return true;

            if (IsMatch("\r\n"))
                return true;

            if (IsMatch("\n"))
                return true;

            return false;
        }
    }
}