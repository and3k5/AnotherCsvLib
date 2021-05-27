using System.Collections.Generic;
using System.IO;
using AnotherCsvLib.Parsing;
using AnotherCsvLib.Parsing.Reader;

// ReSharper disable UnusedMember.Global

namespace AnotherCsvLib
{
    public static partial class Parse
    {
        public static IEnumerable<string[]> ReadAsArrays(TextReader textReader, ParseOptions options = null)
        {
            using (var charReader = new CharReader(textReader))
                return ReadAsArrays(charReader, options);
        }

        internal static IEnumerable<string[]> ReadAsArrays(CharReader charReader, ParseOptions options = null)
        {
            using (var reader = new Parser(charReader, options ?? new ParseOptions()))
                return reader.ReadAsArrays();
        }
    }
}