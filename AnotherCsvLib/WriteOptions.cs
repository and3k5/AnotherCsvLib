using System.Text;

namespace AnotherCsvLib
{
    public sealed class WriteOptions : CsvOptionsBase
    {
        public Encoding Encoding { get; set; } = Encoding.UTF8;
    }
}