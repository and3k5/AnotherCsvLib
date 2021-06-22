using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnotherCsvLib.Parsing.Reader
{
    public sealed class CharReader : IDisposable
    {
        private readonly TextReader _reader;

        public CharReader(TextReader reader)
        {
            _reader = reader;
        }

        private static char? ToChar(int v)
        {
            if (v == -1)
                return null;
            return (char) v;
        }

        private readonly List<char> _cache = new List<char>();

        public char? PeekChar() => PeekChar(0);

        public char? ReadChar()
        {
            lock (_cache)
            {
                // ReSharper disable once InvertIf
                if (_cache.Count > 0)
                {
                    var nextChar = _cache[0];
                    _cache.RemoveAt(0);
                    return nextChar;
                }
            }

            return ToChar(_reader.Read());
        }

        public char? PeekChar(int i)
        {
            if (i < 0) throw new ArgumentOutOfRangeException(nameof(i));

            lock (_cache)
            {
                while (_cache.Count <= i + 1)
                {
                    var readChar = ToChar(_reader.Read());
                    if (!readChar.HasValue)
                        break;
                    _cache.Add(readChar.Value);
                }

                if (_cache.Count > 0 && _cache.Count > i)
                    return _cache[i];
                return null;
            }
        }

        private IEnumerable<char?> PeekChars(int start, int length)
        {
            for (var i = start; i < length; i++)
            {
                yield return PeekChar(i);
            }
        }

        public string PeekString(int start, int length) =>
            new string(PeekChars(start, length).OfType<char>().ToArray());

        public string PeekString(int length) => PeekString(0, length);

        public void SkipChar()
        {
            lock (_cache)
            {
                if (_cache.Count > 0)
                {
                    _cache.RemoveAt(0);
                    return;
                }
            }

            _reader.Read();
        }

        public void Dispose()
        {
            _reader?.Dispose();
        }

        public void SkipChars(int skipChars)
        {
            for (var i = 0; i < skipChars; i++)
            {
                SkipChar();
            }
        }
    }
}