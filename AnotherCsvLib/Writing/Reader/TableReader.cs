using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace AnotherCsvLib.Writing.Reader
{
    internal abstract class TableReader : IDisposable
    {
        public abstract IEnumerable<string> ReadColumns();
        public abstract IEnumerable<RowReader> ReadRows();

        private sealed class TableReaderForDataTableReader : TableReader
        {
            private readonly DataTableReader _reader;

            public TableReaderForDataTableReader(DataTableReader reader)
            {
                if (!reader.CanGetColumnSchema())
                    throw new ArgumentException("DataTableReader should be able to get column schema");

                _reader = reader;
            }

            public override IEnumerable<string> ReadColumns()
            {
                return _reader.GetColumnSchema().Select(x => x.ColumnName);
            }

            public override IEnumerable<RowReader> ReadRows()
            {
                if (!_reader.HasRows)
                    yield break;

                while (_reader.Read())
                {
                    yield return RowReader.Create(_reader);
                }
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _reader?.Dispose();
                }
            }

            public override void Dispose()
            {
                Dispose(true);
            }
        }

        internal static TableReader Create(DataTable dt)
        {
            return new TableReaderForDataTableReader(dt.CreateDataReader());
        }

        public abstract void Dispose();
    }
}