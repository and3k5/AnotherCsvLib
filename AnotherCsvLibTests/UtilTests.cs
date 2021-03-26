using NUnit.Framework;
using System.Data;
using System.IO;

// ReSharper disable once CheckNamespace
namespace AnotherCsvLib.Tests
{
    [TestFixture]
    public class ReadTests
    {
        public Stream SimpleCsvFile()
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.WriteLine("FieldA;FieldB;FieldC;FieldD");
                writer.WriteLine("Foo;Bar;Baz;Yahoo");
                writer.WriteLine("One;Two;Three;Four");
                writer.WriteLine("Five;Six;Seven;Eight");
            }

            stream.Position = 0;
            return stream;
        }

        public Stream LineBreakInLastValueFile(string lineSeparator)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.WriteLine("FieldA;FieldB;FieldC;FieldD");
                writer.WriteLine($"Foo;Bar;Baz;\"Yahoo{lineSeparator}Food\"");
                writer.WriteLine("One;Two;Three;Four");
                writer.WriteLine("Five;Six;Seven;Eight");
            }

            stream.Position = 0;
            return stream;
        }

        public Stream LineBreakInSecondValueFile(string lineSeparator)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.WriteLine("FieldA;FieldB;FieldC;FieldD");
                writer.WriteLine($"Foo;\"Bar{lineSeparator}Food\";Baz;Yahoo");
                writer.WriteLine("One;Two;Three;Four");
                writer.WriteLine("Five;Six;Seven;Eight");
            }

            stream.Position = 0;
            return stream;
        }

        public Stream LineBreakInFirstValueFile(string lineSeparator)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.WriteLine("FieldA;FieldB;FieldC;FieldD");
                writer.WriteLine($"\"Foo{lineSeparator}Food\";Bar;Baz;Yahoo");
                writer.WriteLine("One;Two;Three;Four");
                writer.WriteLine("Five;Six;Seven;Eight");
            }

            stream.Position = 0;
            return stream;
        }

        [Test]
        public void CanReadSimpleCsvFile()
        {
            DataTable dt;
            using (var reader = new StreamReader(SimpleCsvFile()))
            {
                dt = Parse.ReadToDataTable(reader, new ParseOptions());
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
        [TestCase("\r")]
        public void CanReadFileWithAMultiLineFieldInLastValue(string sep)
        {
            DataTable dt;
            using (var reader = new StreamReader(LineBreakInLastValueFile(sep)))
            {
                dt = Parse.ReadToDataTable(reader, new ParseOptions());
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

        [TestCase("\r\n")]
        [TestCase("\n")]
        [TestCase("\r")]
        public void CanReadFileWithAMultiLineFieldInSecondValue(string sep)
        {
            DataTable dt;
            using (var reader = new StreamReader(LineBreakInSecondValueFile(sep)))
            {
                dt = Parse.ReadToDataTable(reader, new ParseOptions());
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

        [TestCase("\r\n")]
        [TestCase("\n")]
        [TestCase("\r")]
        public void CanReadFileWithAMultiLineFieldInFirstValue(string sep)
        {
            DataTable dt;
            using (var reader = new StreamReader(LineBreakInFirstValueFile(sep)))
            {
                dt = Parse.ReadToDataTable(reader, new ParseOptions());
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