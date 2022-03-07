using System;

namespace AnotherCsvLib.Writing.Reader
{
    internal abstract class CellReader : IDisposable
    {
        public abstract object ReadValue();

        public class SimpleCellReader : CellReader
        {
            private readonly object _value;

            public SimpleCellReader(object value)
            {
                _value = value;
            }

            public override object ReadValue()
            {
                return _value;
            }

            public override void Dispose()
            {
            }
        }

        public static CellReader Create(object value)
        {
            return new SimpleCellReader(value);
        }

        public abstract void Dispose();
    }
}