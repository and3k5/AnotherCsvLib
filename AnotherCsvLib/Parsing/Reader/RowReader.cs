using System.Collections.Generic;

namespace AnotherCsvLib.Parsing.Reader
{
    internal class RowReader
    {
        private readonly ValueReader _reader;

        public RowReader(ValueReader reader)
        {
            _reader = reader;
        }

        internal object[] ReadOneRowEnumerable(int rowIndex)
        {
            var row = new List<object>();
            var colIndex = 0;
            while (true)
            {
                var (endRow, value) = _reader.ReadOneValueObject(rowIndex, colIndex++);
                row.Add(value);
                if (endRow)
                    break;
            }

            return row.ToArray();
        }
    }
}