using System.Data;
using System.IO;
using System.Text;
using AnotherCsvLib.Writing;
using AnotherCsvLib.Writing.Reader;

namespace AnotherCsvLib
{
    public class Write
    {
        public static void WriteToFile(string filePath, DataTable dt, WriteOptions options = null) =>
            WriteToFile(filePath, dt, FileMode.CreateNew, options);

        public static void WriteToFile(string filePath, DataTable dt, FileMode fileMode, WriteOptions options = null)
        {
            using (var fileStream = new FileStream(filePath, fileMode))
                WriteToStream(fileStream, dt, options);
        }

        public static void WriteToStream(Stream stream, DataTable dt, WriteOptions options = null)
        {
            options = options ?? new WriteOptions();
            using (var streamWriter = new StreamWriter(stream, options.Encoding))
                WriteToTextWriter(dt, options, streamWriter);
        }

        public static string WriteToString(DataTable dt, WriteOptions options = null)
        {
            var stringBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(stringBuilder))
                WriteToTextWriter(dt, options, stringWriter);
            return stringBuilder.ToString();
        }

        private static void WriteToTextWriter(DataTable dt, WriteOptions options, TextWriter textWriter)
        {
            options = options ?? new WriteOptions();
            using (var writer = new Writer(TableReader.Create(dt)))
                writer.Write(textWriter, options);
        }
    }
}