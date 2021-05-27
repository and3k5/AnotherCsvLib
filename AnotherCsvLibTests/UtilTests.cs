using System;
using NUnit.Framework;
using System.Data;
using System.IO;

// ReSharper disable once CheckNamespace
namespace AnotherCsvLib.Tests
{
    [TestFixture]
    public class ReadTests
    {
        private static Stream CreateStream(string newLine, params string[] lines)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.NewLine = newLine;
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }

            stream.Position = 0;
            return stream;
        }

        [TestCase("\r\n")]
        [TestCase("\n")]
        public void CanReadSimpleCsvFile(string newLine)
        {
            DataTable dt;
            using (var stream = CreateStream(newLine, "FieldA;FieldB;FieldC;FieldD", "Foo;Bar;Baz;Yahoo",
                "One;Two;Three;Four",
                "Five;Six;Seven;Eight"))
            {
                dt = Parse.ReadToDataTable(stream);
            }

            Assert.That(dt, Is.Not.Null);
            Assert.That(dt.Columns.Count, Is.EqualTo(4));
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Rows[0][0], Is.EqualTo("Foo"));
            Assert.That(dt.Rows[0][1], Is.EqualTo("Bar"));
            Assert.That(dt.Rows[0][2], Is.EqualTo("Baz"));
            Assert.That(dt.Rows[0][3], Is.EqualTo("Yahoo"));
            Assert.That(dt.Rows[1][0], Is.EqualTo("One"));
            Assert.That(dt.Rows[1][1], Is.EqualTo("Two"));
            Assert.That(dt.Rows[1][2], Is.EqualTo("Three"));
            Assert.That(dt.Rows[1][3], Is.EqualTo("Four"));
            Assert.That(dt.Rows[2][0], Is.EqualTo("Five"));
            Assert.That(dt.Rows[2][1], Is.EqualTo("Six"));
            Assert.That(dt.Rows[2][2], Is.EqualTo("Seven"));
            Assert.That(dt.Rows[2][3], Is.EqualTo("Eight"));
        }

        [TestCase("\r\n")]
        [TestCase("\n")]
        public void CanReadCsvFileWithEscapedQuote(string newLine)
        {
            DataTable dt;
            using (var stream = CreateStream(newLine, "Id;Name;Price", "100;Bike;10,000",
                "101;\"90\"\" flatscreen\";20,000",
                "102;Shoe;30,000"))
            {
                dt = Parse.ReadToDataTable(stream);
            }

            Assert.That(dt, Is.Not.Null);
            Assert.That(dt.Columns.Count, Is.EqualTo(3));
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Rows[0][0], Is.EqualTo("100"));
            Assert.That(dt.Rows[0][1], Is.EqualTo("Bike"));
            Assert.That(dt.Rows[0][2], Is.EqualTo("10,000"));
            Assert.That(dt.Rows[1][0], Is.EqualTo("101"));
            Assert.That(dt.Rows[1][1], Is.EqualTo("90\" flatscreen"));
            Assert.That(dt.Rows[1][2], Is.EqualTo("20,000"));
            Assert.That(dt.Rows[2][0], Is.EqualTo("102"));
            Assert.That(dt.Rows[2][1], Is.EqualTo("Shoe"));
            Assert.That(dt.Rows[2][2], Is.EqualTo("30,000"));
        }

        [TestCase("\r\n")]
        [TestCase("\n")]
        public void CanReadCsvFileWithEscapedQuoteAsFirstChar(string newLine)
        {
            DataTable dt;
            using (var stream = CreateStream(newLine, "Id;Name;Price", "100;Bike;10,000",
                "101;\"\"\"Really awesome\"\" 90 inch flatscreen\";20,000", "102;Shoe;30,000"))
            {
                dt = Parse.ReadToDataTable(stream);
            }

            Assert.That(dt, Is.Not.Null);
            Assert.That(dt.Columns.Count, Is.EqualTo(3));
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Rows[0][0], Is.EqualTo("100"));
            Assert.That(dt.Rows[0][1], Is.EqualTo("Bike"));
            Assert.That(dt.Rows[0][2], Is.EqualTo("10,000"));
            Assert.That(dt.Rows[1][0], Is.EqualTo("101"));
            Assert.That(dt.Rows[1][1], Is.EqualTo("\"Really awesome\" 90 inch flatscreen"));
            Assert.That(dt.Rows[1][2], Is.EqualTo("20,000"));
            Assert.That(dt.Rows[2][0], Is.EqualTo("102"));
            Assert.That(dt.Rows[2][1], Is.EqualTo("Shoe"));
            Assert.That(dt.Rows[2][2], Is.EqualTo("30,000"));
        }

        [TestCase("\r\n")]
        [TestCase("\n")]
        public void CanReadCsvFileWithEmptyValueAsFirstColumn(string newLine)
        {
            DataTable dt;
            using (var stream = CreateStream(newLine, "Id;Name;Price", "100;Bike;10,000", ";90 inch flatscreen;20,000",
                "102;Shoe;30,000"))
            {
                dt = Parse.ReadToDataTable(stream);
            }

            Assert.That(dt, Is.Not.Null);
            Assert.That(dt.Columns.Count, Is.EqualTo(3));
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Rows[0][0], Is.EqualTo("100"));
            Assert.That(dt.Rows[0][1], Is.EqualTo("Bike"));
            Assert.That(dt.Rows[0][2], Is.EqualTo("10,000"));
            Assert.That(dt.Rows[1][0], Is.EqualTo(DBNull.Value));
            Assert.That(dt.Rows[1][1], Is.EqualTo("90 inch flatscreen"));
            Assert.That(dt.Rows[1][2], Is.EqualTo("20,000"));
            Assert.That(dt.Rows[2][0], Is.EqualTo("102"));
            Assert.That(dt.Rows[2][1], Is.EqualTo("Shoe"));
            Assert.That(dt.Rows[2][2], Is.EqualTo("30,000"));
        }

        [TestCase("\r\n")]
        [TestCase("\n")]
        public void CanReadCsvFileWithEmptyValueAsMidColumn(string newLine)
        {
            DataTable dt;
            using (var stream = CreateStream(newLine, "Id;Name;Price", "100;Bike;10,000", "101;;20,000",
                "102;Shoe;30,000"))
            {
                dt = Parse.ReadToDataTable(stream);
            }

            Assert.That(dt, Is.Not.Null);
            Assert.That(dt.Columns.Count, Is.EqualTo(3));
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Rows[0][0], Is.EqualTo("100"));
            Assert.That(dt.Rows[0][1], Is.EqualTo("Bike"));
            Assert.That(dt.Rows[0][2], Is.EqualTo("10,000"));
            Assert.That(dt.Rows[1][0], Is.EqualTo("101"));
            Assert.That(dt.Rows[1][1], Is.EqualTo(DBNull.Value));
            Assert.That(dt.Rows[1][2], Is.EqualTo("20,000"));
            Assert.That(dt.Rows[2][0], Is.EqualTo("102"));
            Assert.That(dt.Rows[2][1], Is.EqualTo("Shoe"));
            Assert.That(dt.Rows[2][2], Is.EqualTo("30,000"));
        }

        [TestCase("\r\n")]
        [TestCase("\n")]
        public void CanReadCsvFileWithEmptyValueAsLastColumn(string newLine)
        {
            DataTable dt;
            using (var stream = CreateStream(newLine, "Id;Name;Price", "100;Bike;10,000", "101;90 inch flatscreen;",
                "102;Shoe;30,000"))
            {
                dt = Parse.ReadToDataTable(stream);
            }

            Assert.That(dt, Is.Not.Null);
            Assert.That(dt.Columns.Count, Is.EqualTo(3));
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Rows[0][0], Is.EqualTo("100"));
            Assert.That(dt.Rows[0][1], Is.EqualTo("Bike"));
            Assert.That(dt.Rows[0][2], Is.EqualTo("10,000"));
            Assert.That(dt.Rows[1][0], Is.EqualTo("101"));
            Assert.That(dt.Rows[1][1], Is.EqualTo("90 inch flatscreen"));
            Assert.That(dt.Rows[1][2], Is.EqualTo(DBNull.Value));
            Assert.That(dt.Rows[2][0], Is.EqualTo("102"));
            Assert.That(dt.Rows[2][1], Is.EqualTo("Shoe"));
            Assert.That(dt.Rows[2][2], Is.EqualTo("30,000"));
        }

        [TestCase("\r\n")]
        [TestCase("\n")]
        public void CanReadCsvFileWithEmptyNamedColumnAsFirstColumn(string newLine)
        {
            DataTable dt;
            using (var stream = CreateStream(newLine, ";Name;Price", "100;Bike;10,000", "101;90 inch flatscreen;20,000",
                "102;Shoe;30,000"))
            {
                dt = Parse.ReadToDataTable(stream);
            }

            Assert.That(dt, Is.Not.Null);
            Assert.That(dt.Columns.Count, Is.EqualTo(2));
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Rows[0][0], Is.EqualTo("Bike"));
            Assert.That(dt.Rows[0][1], Is.EqualTo("10,000"));
            Assert.That(dt.Rows[1][0], Is.EqualTo("90 inch flatscreen"));
            Assert.That(dt.Rows[1][1], Is.EqualTo("20,000"));
            Assert.That(dt.Rows[2][0], Is.EqualTo("Shoe"));
            Assert.That(dt.Rows[2][1], Is.EqualTo("30,000"));
        }

        [TestCase("\r\n")]
        [TestCase("\n")]
        public void CanReadCsvFileWithColumnSeparatorInsideColumnValue(string newLine)
        {
            DataTable dt;
            using (var stream = CreateStream(newLine, ";Name;Price", "100;Bike;10,000",
                "101;\"90 inch flatscreen;cool\";20,000",
                "102;Shoe;30,000"))
            {
                dt = Parse.ReadToDataTable(stream);
            }

            Assert.That(dt, Is.Not.Null);
            Assert.That(dt.Columns.Count, Is.EqualTo(2));
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Rows[0][0], Is.EqualTo("Bike"));
            Assert.That(dt.Rows[0][1], Is.EqualTo("10,000"));
            Assert.That(dt.Rows[1][0], Is.EqualTo("90 inch flatscreen;cool"));
            Assert.That(dt.Rows[1][1], Is.EqualTo("20,000"));
            Assert.That(dt.Rows[2][0], Is.EqualTo("Shoe"));
            Assert.That(dt.Rows[2][1], Is.EqualTo("30,000"));
        }

        [TestCase("\r\n")]
        [TestCase("\n")]
        public void CanReadCsvFileWithEmptyNamedColumnAsMidColumn(string newLine)
        {
            DataTable dt;
            using (var stream = CreateStream(newLine, "Id;;Price", "100;Bike;10,000", "101;90 inch flatscreen;20,000",
                "102;Shoe;30,000"))
            {
                dt = Parse.ReadToDataTable(stream);
            }

            Assert.That(dt, Is.Not.Null);
            Assert.That(dt.Columns.Count, Is.EqualTo(2));
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Rows[0][0], Is.EqualTo("100"));
            Assert.That(dt.Rows[0][1], Is.EqualTo("10,000"));
            Assert.That(dt.Rows[1][0], Is.EqualTo("101"));
            Assert.That(dt.Rows[1][1], Is.EqualTo("20,000"));
            Assert.That(dt.Rows[2][0], Is.EqualTo("102"));
            Assert.That(dt.Rows[2][1], Is.EqualTo("30,000"));
        }

        [TestCase("\r\n")]
        [TestCase("\n")]
        public void CanReadCsvFileWithEmptyNamedColumnAsLastColumn(string newLine)
        {
            DataTable dt;
            using (var stream = CreateStream(newLine, "Id;Name;", "100;Bike;10,000", "101;90 inch flatscreen;20,000",
                "102;Shoe;30,000"))
            {
                dt = Parse.ReadToDataTable(stream);
            }

            Assert.That(dt, Is.Not.Null);
            Assert.That(dt.Columns.Count, Is.EqualTo(2));
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Rows[0][0], Is.EqualTo("100"));
            Assert.That(dt.Rows[0][1], Is.EqualTo("Bike"));
            Assert.That(dt.Rows[1][0], Is.EqualTo("101"));
            Assert.That(dt.Rows[1][1], Is.EqualTo("90 inch flatscreen"));
            Assert.That(dt.Rows[2][0], Is.EqualTo("102"));
            Assert.That(dt.Rows[2][1], Is.EqualTo("Shoe"));
        }

        [TestCase("\r\n", "\r\n")]
        [TestCase("\r\n", "\n")]
        [TestCase("\n", "\r\n")]
        [TestCase("\n", "\n")]
        [TestCase("\r", "\r\n")]
        [TestCase("\r", "\n")]
        public void CanReadFileWithAMultiLineFieldInLastValue(string sep, string newLine)
        {
            DataTable dt;
            using (var stream = CreateStream(newLine, "FieldA;FieldB;FieldC;FieldD",
                $"Foo;Bar;Baz;\"Yahoo{sep}Food\"",
                "One;Two;Three;Four",
                "Five;Six;Seven;Eight"))
            {
                dt = Parse.ReadToDataTable(stream);
            }

            Assert.That(dt, Is.Not.Null);
            Assert.That(dt.Columns.Count, Is.EqualTo(4));
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Rows[0][0], Is.EqualTo("Foo"));
            Assert.That(dt.Rows[0][1], Is.EqualTo("Bar"));
            Assert.That(dt.Rows[0][2], Is.EqualTo("Baz"));
            Assert.That(dt.Rows[0][3], Is.EqualTo("Yahoo" + sep + "Food"));
            Assert.That(dt.Rows[1][0], Is.EqualTo("One"));
            Assert.That(dt.Rows[1][1], Is.EqualTo("Two"));
            Assert.That(dt.Rows[1][2], Is.EqualTo("Three"));
            Assert.That(dt.Rows[1][3], Is.EqualTo("Four"));
            Assert.That(dt.Rows[2][0], Is.EqualTo("Five"));
            Assert.That(dt.Rows[2][1], Is.EqualTo("Six"));
            Assert.That(dt.Rows[2][2], Is.EqualTo("Seven"));
            Assert.That(dt.Rows[2][3], Is.EqualTo("Eight"));
        }

        [TestCase("\r\n", "\r\n")]
        [TestCase("\r\n", "\n")]
        [TestCase("\n", "\r\n")]
        [TestCase("\n", "\n")]
        [TestCase("\r", "\r\n")]
        [TestCase("\r", "\n")]
        public void CanReadFileWithAMultiLineFieldInSecondValue(string sep, string newLine)
        {
            DataTable dt;
            using (var stream = CreateStream(newLine, "FieldA;FieldB;FieldC;FieldD",
                $"Foo;\"Bar{sep}Food\";Baz;Yahoo",
                "One;Two;Three;Four",
                "Five;Six;Seven;Eight"))
            {
                dt = Parse.ReadToDataTable(stream);
            }

            Assert.That(dt, Is.Not.Null);
            Assert.That(dt.Columns.Count, Is.EqualTo(4));
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Rows[0][0], Is.EqualTo("Foo"));
            Assert.That(dt.Rows[0][1], Is.EqualTo("Bar" + sep + "Food"));
            Assert.That(dt.Rows[0][2], Is.EqualTo("Baz"));
            Assert.That(dt.Rows[0][3], Is.EqualTo("Yahoo"));
            Assert.That(dt.Rows[1][0], Is.EqualTo("One"));
            Assert.That(dt.Rows[1][1], Is.EqualTo("Two"));
            Assert.That(dt.Rows[1][2], Is.EqualTo("Three"));
            Assert.That(dt.Rows[1][3], Is.EqualTo("Four"));
            Assert.That(dt.Rows[2][0], Is.EqualTo("Five"));
            Assert.That(dt.Rows[2][1], Is.EqualTo("Six"));
            Assert.That(dt.Rows[2][2], Is.EqualTo("Seven"));
            Assert.That(dt.Rows[2][3], Is.EqualTo("Eight"));
        }

        [TestCase("\r\n", "\r\n")]
        [TestCase("\r\n", "\n")]
        [TestCase("\n", "\r\n")]
        [TestCase("\n", "\n")]
        [TestCase("\r", "\r\n")]
        [TestCase("\r", "\n")]
        public void CanReadFileWithAMultiLineFieldInFirstValue(string sep, string newLine)
        {
            DataTable dt;
            using (var stream = CreateStream(newLine, "FieldA;FieldB;FieldC;FieldD",
                $"\"Foo{sep}Food\";Bar;Baz;Yahoo",
                "One;Two;Three;Four",
                "Five;Six;Seven;Eight"))
            {
                dt = Parse.ReadToDataTable(stream);
            }

            Assert.That(dt, Is.Not.Null);
            Assert.That(dt.Columns.Count, Is.EqualTo(4));
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Rows[0][0], Is.EqualTo("Foo" + sep + "Food"));
            Assert.That(dt.Rows[0][1], Is.EqualTo("Bar"));
            Assert.That(dt.Rows[0][2], Is.EqualTo("Baz"));
            Assert.That(dt.Rows[0][3], Is.EqualTo("Yahoo"));
            Assert.That(dt.Rows[1][0], Is.EqualTo("One"));
            Assert.That(dt.Rows[1][1], Is.EqualTo("Two"));
            Assert.That(dt.Rows[1][2], Is.EqualTo("Three"));
            Assert.That(dt.Rows[1][3], Is.EqualTo("Four"));
            Assert.That(dt.Rows[2][0], Is.EqualTo("Five"));
            Assert.That(dt.Rows[2][1], Is.EqualTo("Six"));
            Assert.That(dt.Rows[2][2], Is.EqualTo("Seven"));
            Assert.That(dt.Rows[2][3], Is.EqualTo("Eight"));
        }
    }
}