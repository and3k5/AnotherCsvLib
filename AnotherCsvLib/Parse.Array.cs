using System.Collections.Generic;
using System.IO;
using AnotherCsvLib.Parsing;

// ReSharper disable UnusedMember.Global

namespace AnotherCsvLib
{
    public static partial class Parse
    {
        public static IEnumerable<string[]> ReadAsArrays(TextReader textReader, ParseOptions options = null)
        {
            using (var reader = new Parser(textReader, options ?? new ParseOptions()))
                return reader.ReadAsArrays();
        }
    }
}