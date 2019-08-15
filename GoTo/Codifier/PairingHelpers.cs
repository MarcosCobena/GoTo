using System;
using System.Numerics;

namespace GoTo.Codifier
{
    public static class PairingHelpers
    {
        public static int Pair(int a, int b) => 
            ((int)BigInteger.Pow(2, a) * ((2 * b) + 1)) - 1;

        // Translated from:
        // https://github.com/j-k/number-pairings/blob/master/src/number-pairings.js#L64
        public static (int a, int b) Unpair(int c)
        {
            int a;
            var cQuote = c + 1;

            for (int i = 0; i < cQuote; i++)
            {
                a = i;
                var d = (int)BigInteger.Pow(2, a);
                var e = cQuote / d;

                if ((e % 2) == 1)
                {
                    var b = e / 2;

                    return (a, b);
                }
            }

            throw new InvalidOperationException();
        }
    }
}