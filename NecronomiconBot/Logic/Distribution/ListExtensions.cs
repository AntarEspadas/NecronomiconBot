using System;
using System.Collections.Generic;
using System.Text;

namespace NecronomiconBot.Logic.Distribution
{
    public static class ListExtensions
    {
        private static readonly Random random = new Random();
        public static void Derrange<T>(this IList<T> list)
        {
            DiceRoll p = new DiceRoll(0);
            bool[] marked = new bool[list.Count];
            long[] d = DistributionMath.AllSubfactorials(list.Count);
            int j;
            for (int i = list.Count - 1, u = list.Count; u >= 2; i--)
            {
                if (!marked[i])
                {
                    do
                    {
                        j = random.Next(0, i);
                    } while (marked[j]);
                    T temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                    p.Percentage = (u - 1) * (d[u - 2] / (float)d[u]) * 100;
                    if (p.Roll())
                    {
                        marked[j] = true;
                        u--;
                    }
                    u--;
                }
            }
        }
    }
}
