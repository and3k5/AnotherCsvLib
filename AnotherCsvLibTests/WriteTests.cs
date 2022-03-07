using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AnotherCsvLib.Tests
{
    [TestFixture]
    public class WriteTests
    {
        public static IEnumerable<object[]> GetWriteOptions()
        {
            IEnumerable<WriteOptions> IterateWriteOptions()
            {
                foreach (var quoteChar in new[] { '"', '\'' })
                foreach (var columnSeparator in new[] { ',', ';' })
                    yield return new WriteOptions
                    {
                        QuoteChar = quoteChar,
                        ColumnSeparator = columnSeparator,
                        Encoding = Encoding.UTF8,
                    };
            }

            return IterateWriteOptions().Select(x => new object[] { x });
        }

        [TestCaseSource(nameof(GetWriteOptions))]
        public void CanWriteBasicTableToString(WriteOptions options)
        {
            var dt = new DataTable();
            dt.InsertColumns("id", "name", "info");
            dt.InsertRow(1, "foo", "this is the first row");
            dt.InsertRow(2, "bar", "this is the second row");

            var content = Write.WriteToString(dt, options);

            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            Assert.That(lines.Length, Is.EqualTo(3));

            Assert.That(lines[0], Is.EqualTo($"id{options.ColumnSeparator}name{options.ColumnSeparator}info"));
            Assert.That(lines[1],
                Is.EqualTo($"1{options.ColumnSeparator}foo{options.ColumnSeparator}this is the first row"));
            Assert.That(lines[2],
                Is.EqualTo($"2{options.ColumnSeparator}bar{options.ColumnSeparator}this is the second row"));
        }

        [TestCaseSource(nameof(GetWriteOptions))]
        public void CanWriteTableWithQuotesInColumnNameToString(WriteOptions options)
        {
            var dt = new DataTable();
            dt.InsertColumns("id", "name", $"special{options.QuoteChar}");
            dt.InsertRow(1, "foo", "this is the first row");
            dt.InsertRow(2, "bar", "this is the second row");

            var content = Write.WriteToString(dt, options);

            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            Assert.That(lines.Length, Is.EqualTo(3));

            Assert.That(lines[0], Is.EqualTo(
                $"id{options.ColumnSeparator}name{options.ColumnSeparator}{options.QuoteChar}special{options.QuoteChar}{options.QuoteChar}{options.QuoteChar}"));
            Assert.That(lines[1],
                Is.EqualTo($"1{options.ColumnSeparator}foo{options.ColumnSeparator}this is the first row"));
            Assert.That(lines[2],
                Is.EqualTo($"2{options.ColumnSeparator}bar{options.ColumnSeparator}this is the second row"));
        }

        [TestCaseSource(nameof(GetWriteOptions))]
        public void CanWriteTableWithQuotesInRowValueNameToString(WriteOptions options)
        {
            var dt = new DataTable();
            dt.InsertColumns("id", "name", "info");
            dt.InsertRow(1, "foo", $"this is the {options.QuoteChar}first{options.QuoteChar} row");
            dt.InsertRow(2, "bar", "this is the second row");

            var content = Write.WriteToString(dt, options);

            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            Assert.That(lines.Length, Is.EqualTo(3));

            Assert.That(lines[0], Is.EqualTo($"id{options.ColumnSeparator}name{options.ColumnSeparator}info"));
            Assert.That(lines[1], Is.EqualTo(
                $"1{options.ColumnSeparator}foo{options.ColumnSeparator}{options.QuoteChar}this is the {options.QuoteChar}{options.QuoteChar}first{options.QuoteChar}{options.QuoteChar} row{options.QuoteChar}"));
            Assert.That(lines[2],
                Is.EqualTo($"2{options.ColumnSeparator}bar{options.ColumnSeparator}this is the second row"));
        }

        [TestCaseSource(nameof(GetWriteOptions))]
        public void CanWriteTableWithColumnSeparatorInColumnNameToString(WriteOptions options)
        {
            var dt = new DataTable();
            dt.InsertColumns("id", "name", $"special{options.ColumnSeparator}stuff");
            dt.InsertRow(1, "foo", "this is the first row");
            dt.InsertRow(2, "bar", "this is the second row");

            var content = Write.WriteToString(dt, options);

            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            Assert.That(lines.Length, Is.EqualTo(3));

            Assert.That(lines[0], Is.EqualTo(
                $"id{options.ColumnSeparator}name{options.ColumnSeparator}{options.QuoteChar}special{options.ColumnSeparator}stuff{options.QuoteChar}"));
            Assert.That(lines[1], Is.EqualTo(
                $"1{options.ColumnSeparator}foo{options.ColumnSeparator}this is the first row"));
            Assert.That(lines[2], Is.EqualTo(
                $"2{options.ColumnSeparator}bar{options.ColumnSeparator}this is the second row"));
        }

        [TestCaseSource(nameof(GetWriteOptions))]
        public void CanWriteTableWithColumnSeparatorInRowValueNameToString(WriteOptions options)
        {
            var dt = new DataTable();
            dt.InsertColumns("id", "name", "info");
            dt.InsertRow(1, "foo", $"this is the first row{options.ColumnSeparator} you guys");
            dt.InsertRow(2, "bar", "this is the second row");

            var content = Write.WriteToString(dt, options);

            var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            Assert.That(lines.Length, Is.EqualTo(3));

            Assert.That(lines[0], Is.EqualTo($"id{options.ColumnSeparator}name{options.ColumnSeparator}info"));
            Assert.That(lines[1], Is.EqualTo(
                $"1{options.ColumnSeparator}foo{options.ColumnSeparator}{options.QuoteChar}this is the first row{options.ColumnSeparator} you guys{options.QuoteChar}"));
            Assert.That(lines[2],
                Is.EqualTo($"2{options.ColumnSeparator}bar{options.ColumnSeparator}this is the second row"));
        }
    }
}