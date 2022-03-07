namespace AnotherCsvLib
{
    public abstract class CsvOptionsBase
    {
        public char QuoteChar { get; set; } = '"';
        public char ColumnSeparator { get; set; } = ';';
    }
}