using System;
using System.Collections.Generic;
using System.Data;

namespace AnotherCsvLib.Writing.Reader
{
    internal abstract class RowReader : IDisposable
    {
        public abstract IEnumerable<CellReader> ReadCells();

        private class RowReaderForDataTableReader : RowReader
        {
            private readonly DataTableReader _reader;

            public RowReaderForDataTableReader(DataTableReader reader)
            {
                _reader = reader;
            }

            public override IEnumerable<CellReader> ReadCells()
            {
                for (var i = 0; i < _reader.FieldCount; i++)
                    yield return CellReader.Create(_reader.GetValue(i));
            }

            public override void Dispose()
            {
            }
        }

        public static RowReader Create(DataTableReader reader) => new RowReaderForDataTableReader(reader);

        public abstract void Dispose();
    }
}