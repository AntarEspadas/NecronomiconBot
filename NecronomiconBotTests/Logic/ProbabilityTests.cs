using Microsoft.VisualStudio.TestTools.UnitTesting;
using NecronomiconBot.Logic;

namespace NecronomiconBotTests.Loigc
{
    [TestClass]
    public class ProbabilityTests
    {
        [TestMethod]
        public void TestFactorial()
        {
            Assert.AreEqual(1, Probability.Factorial(0));
            Assert.AreEqual(1, Probability.Factorial(1));
            Assert.AreEqual(2, Probability.Factorial(2));
            Assert.AreEqual(6, Probability.Factorial(3));

            Assert.AreEqual(Probability.Factorial(10), Probability.Factorial(10, 5, Probability.Factorial(5)));
            Assert.AreEqual(Probability.Factorial(10), Probability.Factorial(10, 3, Probability.Factorial(3)));
            Assert.AreEqual(Probability.Factorial(10), Probability.Factorial(10, 7, Probability.Factorial(7)));
        }

        [TestMethod]
        public void TestSubFactorial()
        {
            double g = 1;
            long f = 1;
            Assert.AreEqual(0, Probability.Subfactorial(1, 0, ref g, ref f));
            Assert.AreEqual(1, Probability.Subfactorial(2, 1, ref g, ref f));
            g = 1;
            f = 1;
            Assert.AreEqual(1, Probability.Subfactorial(2, 0, ref g, ref f));
            g = 1;
            f = 1;
            long subf = Probability.Subfactorial(10, 0, ref g, ref f);
            Assert.AreEqual(1334961, subf);

            long[] expected = {1, 0, 1, 2, 9, 44, 265};
            long[] actual = Probability.AllSubfactorials(6);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void TestDerrange()
        {
            int[] list = new int[10];
            int[] listCopy = new int[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = i;
            }
            for (int i = 0; i <= 50; i++)
            {
                list.CopyTo(listCopy, 0);
                Probability.Derrange(listCopy);
                for (int j = 0; j < list.Length; j++)
                {
                    Assert.AreNotEqual(list[j], listCopy[j]);
                }
            }
        }
    }
}
