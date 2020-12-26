using System;
using System.Collections.Generic;
using System.Text;

namespace NecronomiconBot.Logic.Distribution
{
    public static class DistributionMath
    {
        public static long[] AllSubfactorials(int n)
        {
            long[] result = new long[n + 1];
            result[0] = 1;
            double incompleteGamma = 1;
            long factorial = 1;
            for (int i = 1; i <= n; i++)
            {
                result[i] = Subfactorial(i, i - 1, ref incompleteGamma, ref factorial);
            }
            return result;
        }

        public static long Subfactorial(int n, int previousN, ref double incompleteGamma, ref long factorial)
        {
            long SubFactorial;

            for (int i = previousN + 1; i <= n; i++)
            {
                factorial = Factorial(i, i - 1, factorial);
                incompleteGamma += Math.Pow(-1, i) / factorial;
            }
            SubFactorial = (long)Math.Round(factorial * incompleteGamma);

            return SubFactorial;
        }

        public static long Factorial(int n)
        {
            if (n == 0)
                return 1;
            return Factorial(n, 0, 1);
        }

        public static long Factorial(int n, int previousN, long previousFactorial)
        {
            if (n < 0 || previousN < 0)
                throw new ArithmeticException("Cannont calculate the factorial of a negative");
            if (previousN >= n)
                throw new ArithmeticException("Argument previousN must be less than n");
            if (previousFactorial < previousN)
                throw new ArithmeticException("previousFactorial cannot be less than previousN");
            long factorial = previousFactorial;
            for (int i = previousN + 1; i <= n; i++)
            {
                factorial *= i;
            }
            return factorial;
        }
    }
}
