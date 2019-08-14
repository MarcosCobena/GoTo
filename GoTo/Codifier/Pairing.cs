using System;

namespace GoTo.Codifier
{
    public static class Pairing
    {
        public static double Pair(double a, double b) => 
            (Math.Pow(2, a) * ((2 * b) + 1)) - 1;

        // Translated from:
        // https://github.com/j-k/number-pairings/blob/master/src/number-pairings.js#L64
        public static (double a, double b) Unpair(double c)
        {
            double a;
            var cQuote = c + 1;

            for (int i = 0; i < cQuote; i++)
            {
                a = i;
                var d = Math.Pow(2, a);
                var e = cQuote / d;

                if ((e % 2) == 1)
                {
                    var b = Math.Floor(e / 2);

                    return (a, b);
                }
            }

            throw new InvalidOperationException();
        }
    }
}