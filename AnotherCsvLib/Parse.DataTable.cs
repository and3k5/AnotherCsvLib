﻿using System.Data;
using System.IO;
using AnotherCsvLib.Parsing;
using AnotherCsvLib.Parsing.Reader;

namespace AnotherCsvLib
{
    public static partial class Parse
    {
        public static DataTable ReadToDataTable(string content, ParseOptions options = null)
        {
            using (var stringReader = new StringReader(content))
                return ReadToDataTable(stringReader, options);
        }

        public static DataTable ReadFileToDataTable(string filePath, ParseOptions options = null)
        {
            return ReadToDataTable(new FileInfo(filePath), options);
        }

        public static DataTable ReadToDataTable(FileInfo file, ParseOptions options = null)
        {
            using (var stream = file.Open(FileMode.Open, FileAccess.Read))
                return ReadToDataTable(stream, options);
        }

        public static DataTable ReadToDataTable(Stream stream, ParseOptions options = null)
        {
            using (var streamReader = new StreamReader(stream))
                return ReadToDataTable(streamReader, options);
        }

        public static DataTable ReadToDataTable(TextReader textReader, ParseOptions options = null)
        {
            using (var charReader = new CharReader(textReader))
                return ReadToDataTable(charReader, options);
        }

        public static event ParsedDataTableDelegate ParsedDataTable;

        internal static DataTable ReadToDataTable(CharReader charReader, ParseOptions options = null)
        {
            using (var reader = new Parser(charReader, options ?? new ParseOptions()))
            {
                reader.ParsedDataTable += ParsedDataTable;
                return reader.ReadAsDataTable();
            }
        }
    }

    public delegate void ParsedDataTableDelegate(DataTable dataTable);
}