using System;
using System.Collections.Generic;

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

        internal (bool endRow, object value) ReadOneValueObject()
        {
            var insideQuotedValue = false;

            var chars = new List<char>();
            var endRow = false;
            var nextI = 0;

            do
            {
                var i = nextI++;
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
                        chars.Add(_options.QuoteChar);
                        _reader.SkipChar();
                        continue;
                    }

                    if (!insideQuotedValue)
                        throw new InvalidOperationException("Invalid CSV content");

                    ch = _reader.ReadChar();

                    if (ch == null)
                    {
                        endRow = true;
                        break;
                    }

                    nextCh = _reader.PeekChar();

                    if (CheckEndingCharacters(ch, nextCh, ref endRow))
                        break;

                    throw new InvalidOperationException("Invalid end of column value");
                }

                if (!insideQuotedValue)
                {
                    if (CheckEndingCharacters(ch, nextCh, ref endRow))
                        break;
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

        private bool CheckEndingCharacters(char? ch, char? nextCh, ref bool endRow1)
        {
            if (ch == _options.ColumnSeparator)
                return true;

            switch (ch)
            {
                case '\r' when nextCh == '\n':
                    _reader.SkipChar();
                    endRow1 = true;
                    return true;
                case '\n':
                    endRow1 = true;
                    return true;
                default:
                    return false;
            }
        }
    }
}