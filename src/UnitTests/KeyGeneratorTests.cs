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
            Assert.AreEqual(KeyGenerator.SequenceStatus.Valid, _Generator.Validate("ATCGACGATGATATGAGTATCACGTGCTCAGCGCA", "", 0.5, 8, 3, 1), "This should be a valid sequence");
        }

        [TestMethod]
        //The first number is arbitrary. It is just to make it easy to find which sequence failed
        [DataRow(1, "AACGACGATGATATGAGTATCACGTGCTCAGCGCA", KeyGenerator.SequenceStatus.Homopolymer)]
        [DataRow(2, "ATCGACGATGATATGAGTATCACGTGCTCAGCAAA", KeyGenerator.SequenceStatus.Homopolymer)] // homopolymers
        [DataRow(3, "ATCGACGATGATAGGAGTATCACGTGCTCAGCGCA", KeyGenerator.SequenceStatus.Homopolymer)] // homopolymers
        [DataRow(4, "CTCGACGACGACGTGCGCGTCACGTGCTCAGCGCA", KeyGenerator.SequenceStatus.GCContent)] // GC Content
        [DataRow(5, "ATCGACGATGATATGAGTATCACGTAGCTGCTACT", KeyGenerator.SequenceStatus.Folding)] // Folding
        [DataRow(6, "ATCGACGATGATATGAGTATCAGCTGCTACTCGCA", KeyGenerator.SequenceStatus.Folding)] // Folding 
        [DataRow(7, "ATCGACGATGATATGAGTATCACGTGTAGCTGCTA", KeyGenerator.SequenceStatus.Folding)] // Folding
        [DataRow(8, "ATCGACGATGATGATAGTATCACGTGCTCAGCGCA", KeyGenerator.SequenceStatus.Repeats)] // Repeated Sequence
        [DataRow(9, "ATCGACGATGATATGAGTATCACGTGCTCAGCGAT", KeyGenerator.SequenceStatus.Repeats)] // Repeated Sequence
        [DataRow(10, "ATCGACGATGATATGAGTATCACGTGCTCAGATCA", KeyGenerator.SequenceStatus.Repeats)] // Repeated Sequence
        [DataRow(11, "ATATATGATGATATGAGTATCACGTGCTCAGCGCA", KeyGenerator.SequenceStatus.Repeats)] // Repeated Sequence
        public void TestMethodValidateFail(int index, string str, KeyGenerator.SequenceStatus status)
        {
            Assert.AreEqual(status, _Generator.Validate(str, "", 0.5, 8, 3, 1), "Sequence {0} should fail", index);
        }

    }
}