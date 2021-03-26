using System.Data;
using System.IO;
using AnotherCsvLib.Parsing;

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
            using (var reader = new Parser(textReader, options ?? new ParseOptions()))
                return reader.ReadAsDataTable();
        }
    }
}