using NUnit.Framework;
using System;
using System.IO;

// ReSharper disable once CheckNamespace
namespace AnotherCsvLib.Parsing.Reader.Tests
{
    [TestFixture]
    public class CharReaderTests
    {
        [Test]
        public void CanReadSimpleString()
        {
            using var textReader = new StringReader("test");
            using var charReader = new CharReader(textReader);

            Assert.That(charReader.PeekChar(), Is.EqualTo('t'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('t'));
            Assert.That(charReader.PeekChar(), Is.EqualTo('e'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('e'));
            Assert.That(charReader.PeekChar(), Is.EqualTo('s'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('s'));
            Assert.That(charReader.PeekChar(), Is.EqualTo('t'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('t'));
            Assert.That(charReader.PeekChar(), Is.Null);
            Assert.That(charReader.ReadChar(), Is.Null);
        }

        [TestCase("Hello world!")]
        [TestCase("H")]
        [TestCase("Loooooooooooooooooooooooooooooooooooooooooong test")]
        public void CanReadSimpleContent(string testString)
        {
            using var textReader = new StringReader(testString);
            using var charReader = new CharReader(textReader);


            foreach (var ch in testString)
            {
                var peekedChar = charReader.PeekChar();
                Assert.That(peekedChar, Is.EqualTo(ch));
                var readChar = charReader.ReadChar();
                Assert.That(readChar, Is.EqualTo(ch));
            }

            Assert.That(charReader.PeekChar(), Is.Null);
            Assert.That(charReader.ReadChar(), Is.Null);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        public void CanReadSimpleContentWhileCaching(int readAhead)
        {
            const string testString = "Hello world!";
            using var textReader = new StringReader(testString);
            using var charReader = new CharReader(textReader);


            Assert.That(charReader.PeekString(readAhead),
                Is.EqualTo(testString.Substring(0, Math.Min(testString.Length, readAhead))));
            Assert.That(charReader.PeekChar(), Is.EqualTo('H'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('H'));
            Assert.That(charReader.PeekChar(), Is.EqualTo('e'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('e'));
            Assert.That(charReader.PeekChar(), Is.EqualTo('l'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('l'));
            Assert.That(charReader.PeekChar(), Is.EqualTo('l'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('l'));
            Assert.That(charReader.PeekChar(), Is.EqualTo('o'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('o'));
            Assert.That(charReader.PeekChar(), Is.EqualTo(' '));
            Assert.That(charReader.ReadChar(), Is.EqualTo(' '));
            Assert.That(charReader.PeekString(6), Is.EqualTo("world!"));
            Assert.That(charReader.PeekChar(), Is.EqualTo('w'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('w'));
            Assert.That(charReader.PeekChar(), Is.EqualTo('o'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('o'));
            Assert.That(charReader.PeekChar(), Is.EqualTo('r'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('r'));
            Assert.That(charReader.PeekChar(), Is.EqualTo('l'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('l'));
            Assert.That(charReader.PeekChar(), Is.EqualTo('d'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('d'));
            Assert.That(charReader.PeekChar(), Is.EqualTo('!'));
            Assert.That(charReader.ReadChar(), Is.EqualTo('!'));

            Assert.That(charReader.PeekChar(), Is.Null);
            Assert.That(charReader.ReadChar(), Is.Null);
        }
    }
}