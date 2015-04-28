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

        // Format: First X Digit represent e, Last 15 Digit represent phi
        public string GeneratePublicKey()
        {
            ulong first = generator.GetPrime();
            ulong second = generator.GetPrime();
            ulong mult = (first * second);
            ulong phi =  (first - 1) * (second -1);
            ulong e = generator.GetCoPrime(phi);

            string padding = "";
            for (int i = 0; i < 10-e.ToString().Length; i++)
            {
                padding += "0";
            }

            //Console.WriteLine(padding + e.ToString() + " | " + phi.ToString());

            // (e, phi)
            return padding + e.ToString() + phi.ToString();

            //Console.WriteLine("First: " + first.ToString() + " Second: " + second.ToString() +
            //    " Multiplier: " + mult + " Q: " + phi + " CoPrime: " + e + " D: " + d);
        }

        public string GeneratePrivateKey(string publicKey)
        {
            string sE = publicKey.Substring(0, 10);
            string sPhi = publicKey.Substring(10);
            ulong e = Convert.ToUInt64(sE);
            ulong phi = Convert.ToUInt64(sPhi);
            ulong d = modInverse(e, phi);

            string padding = "";
            for (int i = 0; i < 20-d.ToString().Length; i++)
            {
                padding += "0";
            }

            // (d, phi)
            return padding + d.ToString() + phi.ToString() ;
        }
    }
}
