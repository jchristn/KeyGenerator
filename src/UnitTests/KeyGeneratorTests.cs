using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Generator;

namespace UnitTests
{
    [TestClass]
    public class KeyGeneratorTests
    {
        KeyGenerator _Generator = new KeyGenerator();

        public KeyGeneratorTests()
        {
            _Generator.Logger = Console.WriteLine;
        }

        [TestMethod]
        public void TestMethodGenerate()
        {
            (string, int) ret = new();
            ret = _Generator.Generate(35, 1000, false, 0.5, 8, 3, 1);
            Assert.IsNotNull(ret.Item1, "Generate key returned null");
        }

        [TestMethod]
        public void TestMethodValidatePass() 
        {
            Assert.AreEqual(Generator.SequenceStatus.Valid, _Generator.Validate("ATCGACGATGATATGAGTATCACGTGCTCAGCGCA", "", 0.5, 8, 3, 1), "This should be a valid sequence");
        }

        [TestMethod]
        //The first number is arbitrary. It is just to make it easy to find which sequence failed
        [DataRow(1, "AACGACGATGATATGAGTATCACGTGCTCAGCGCA", Generator.SequenceStatus.Homopolymer)]
        [DataRow(2, "ATCGACGATGATATGAGTATCACGTGCTCAGCAAA", Generator.SequenceStatus.Homopolymer)] // homopolymers
        [DataRow(3, "ATCGACGATGATAGGAGTATCACGTGCTCAGCGCA", Generator.SequenceStatus.Homopolymer)] // homopolymers
        [DataRow(4, "CTCGACGACGACGTGCGCGTCACGTGCTCAGCGCA", Generator.SequenceStatus.GCContent)] // GC Content
        [DataRow(5, "ATCGACGATGATATGAGTATCACGTAGCTGCTACT", Generator.SequenceStatus.Folding)] // Folding
        [DataRow(6, "ATCGACGATGATATGAGTATCAGCTGCTACTCGCA", Generator.SequenceStatus.Folding)] // Folding 
        [DataRow(7, "ATCGACGATGATATGAGTATCACGTGTAGCTGCTA", Generator.SequenceStatus.Folding)] // Folding
        [DataRow(8, "ATCGACGATGATGATAGTATCACGTGCTCAGCGCA", Generator.SequenceStatus.Repeats)] // Repeated Sequence
        [DataRow(9, "ATCGACGATGATATGAGTATCACGTGCTCAGCGAT", Generator.SequenceStatus.Repeats)] // Repeated Sequence
        [DataRow(10, "ATCGACGATGATATGAGTATCACGTGCTCAGATCA", Generator.SequenceStatus.Repeats)] // Repeated Sequence
        [DataRow(11, "ATATATGATGATATGAGTATCACGTGCTCAGCGCA", Generator.SequenceStatus.Repeats)] // Repeated Sequence
        public void TestMethodValidateFail(int index, string str, Generator.SequenceStatus status)
        {
            Assert.AreEqual(status, _Generator.Validate(str, "", 0.5, 8, 3, 1), "Sequence {0} should fail", index);
        }
    }
}