using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            private readonly DataTable _dataTable;

            public TableReaderForDataTableReader(DataTableReader reader, DataTable dataTable)
            {
                _reader = reader;
                _dataTable = dataTable;
            }

            public override IEnumerable<string> ReadColumns()
            {
                if (!_reader.CanGetColumnSchema())
                {
                    return _dataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName);
                }
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
            var dataTableReader = dt.CreateDataReader();
            return new TableReaderForDataTableReader(dataTableReader, dt);
        }

        public abstract void Dispose();
    }
}