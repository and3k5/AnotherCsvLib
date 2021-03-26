namespace AnotherCsvLib
{
    public sealed class ParseOptions
    {
        public char QuoteChar { get; set; } = '"';

        public char ColumnSeparator { get; set; } = ';';
    }
}