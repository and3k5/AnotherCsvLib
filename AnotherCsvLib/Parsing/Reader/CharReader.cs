using System;
using System.IO;

namespace AnotherCsvLib.Parsing.Reader
{
    internal class CharReader : IDisposable
    {
        private readonly TextReader _reader;

        public CharReader(TextReader reader)
        {
            _reader = reader;
        }

        protected char? ToChar(int v)
        {
            if (v == -1)
                return null;
            return (char) v;
        }

        public char? PeekChar() => ToChar(_reader.Peek());

        public char? ReadChar() => ToChar(_reader.Read());

        public void SkipChar() => _reader.Read();

        public void Dispose()
        {
            _reader?.Dispose();
        }
    }
}