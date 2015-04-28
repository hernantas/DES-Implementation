using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESEncryption
{
    public class RSA
    {
        private PrimeGenerator generator = null;
        private string key = "";
        public string Key
        {
            get;
            set;
        }

        public RSA()
        {
            generator = new PrimeGenerator();
        }

        Tuple<ulong, Tuple<ulong, ulong>> extendedEuclid(ulong a, ulong b) 
        {
            ulong x = 1, y = 0;
            ulong xLast = 0, yLast = 1;
            ulong q, r, m, n;

            while(a != 0) 
            {
                q = b / a;
                r = b % a;
                m = xLast - q * x;
                n = yLast - q * y;
                xLast = x; 
                yLast = y;
                x = m;
                y = n;
                b = a;
                a = r;
            }
            return new Tuple<ulong, Tuple<ulong, ulong>>(b, new Tuple<ulong, ulong>(xLast, yLast));
        }

        ulong modInverse(ulong a, ulong m)
        {
            return (extendedEuclid(a, m).Item2.Item1 + m) % m;
        }

        public string GenerateKey()
        {
            ulong first = generator.GetPrime();
            ulong second = generator.GetPrime();
            ulong mult = (first * second);
            ulong phi =  (first - 1) * (second -1);
            ulong e = generator.GetCoPrime(phi);
            ulong d = modInverse(e, phi);

            Console.WriteLine("First: " + first.ToString() + " Second: " + second.ToString() +
                " Multiplier: " + mult + " Q: " + phi + " CoPrime: " + e + " D: " + d);
            return "First: " + first.ToString() + " Second: " + second.ToString() +
                " Multiplier: " + mult + " Q: " + phi + " CoPrime: " + e + " D: " + d;
        }
    }
}
