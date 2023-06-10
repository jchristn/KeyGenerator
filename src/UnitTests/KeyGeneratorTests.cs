using System.ComponentModel.DataAnnotations;
using Generator;

namespace UnitTests
{
    [TestClass]
    public class KeyGeneratorTests
    {
        [TestMethod]
        public void TestMethodGenerate()
        {
            KeyGenerator _generator = new KeyGenerator();
            (string, int) ret = new();
            ret = _generator.Generate(35, 1000, false, 0.5, 8, 3, 1);
            Assert.IsNotNull(ret.Item1, "Generate key returned null");
        }
    }
}