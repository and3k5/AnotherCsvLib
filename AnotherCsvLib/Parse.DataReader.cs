using System.Data;
using System.IO;

// ReSharper disable UnusedMember.Global

namespace AnotherCsvLib
{
    public static partial class Parse
    {
        public static IDataReader ReadToDataReader(string content, ParseOptions options = null)
        {
            return ReadToDataTable(content, options).CreateDataReader();
        }

        public static IDataReader ReadFileToDataReader(string filePath, ParseOptions options = null)
        {
            return ReadFileToDataTable(filePath, options).CreateDataReader();
        }

        public static IDataReader ReadToDataReader(FileInfo file, ParseOptions options = null)
        {
            return ReadToDataTable(file, options).CreateDataReader();
        }

        public static IDataReader ReadToDataReader(Stream stream, ParseOptions options = null)
        {
            return ReadToDataTable(stream, options).CreateDataReader();
        }

        public static IDataReader ReadToDataReader(TextReader textReader, ParseOptions options = null)
        {
            return ReadToDataTable(textReader, options).CreateDataReader();
        }
    }
}