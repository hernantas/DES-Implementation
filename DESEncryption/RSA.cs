using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;

namespace DESEncryption
{
    public class RSA
    {
        private PrimeGenerator generator = new PrimeGenerator();
        private RSAKey key = new RSAKey();
        public RSAKey Key
        {
            set { key = value; }
            get { return key; }
        }

        public RSA()
        {
        }

        public RSA(RSAKey key)
        {
            this.key = key;
        }

        Tuple<BigInteger, Tuple<BigInteger, BigInteger>> extendedEuclid(BigInteger a, BigInteger b) 
        {
            BigInteger x = 1, y = 0;
            BigInteger xLast = 0, yLast = 1;
            BigInteger q, r, m, n;

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

            return new Tuple<BigInteger, Tuple<BigInteger, BigInteger>>(b, new Tuple<BigInteger, BigInteger>(xLast, yLast));
        }

        private BigInteger modInverse(BigInteger a, BigInteger m)
        {
            return (extendedEuclid(a, m).Item2.Item1 + m) % m;
        }

        public RSAKey GenerateKey()
        {
            BigInteger first = generator.FindPrime();
            BigInteger second = generator.FindPrime();
            // BigInteger first = 17;
            // BigInteger second = 11; 

            while (first == second)
                second = generator.FindPrime();

            BigInteger n = (first * second);
            BigInteger phi = (first - 1) * (second - 1);
            BigInteger e = generator.FindCoPrime(phi);
            BigInteger d = modInverse(e, phi);

            key = new RSAKey();
            key.n = n;
            key.e = e;
            key.d = d;

            Console.WriteLine("first: " + first);
            Console.WriteLine("second: " + second);
            Console.WriteLine("pxq: " + n);
            Console.WriteLine("(p-1)x(q-1): " + phi);
            Console.WriteLine("e: " + e);
            Console.WriteLine("d: " + d);

            return key;
        }

        public string encrypt(string message)
        {
            int[] block = UtilityConverter.StringToNumBlock(message);

            string hex = "";

            for (int i = 0; i < block.Length; i++)
            {
                BigInteger enc = BigInteger.ModPow(block[i], key.e, key.n);
                string hexTmp = enc.ToString("x");

                //Console.WriteLine(enc + " to " + hexTmp + "("+hexTmp.Length+")");

                if (hex == "")
                    hex = hexTmp;
                else
                    hex += "g" + hexTmp;
            }

            //Console.WriteLine();

            return hex;
        }

        public string decypt(string hex)
        {
            string[] hexBlock = hex.Split('g');
            int length = hexBlock.Length;

            int[] numblock = new int[length];

            int i = 0;
            foreach (string s in hexBlock)
            {
                BigInteger bint = BigInteger.Parse(s, NumberStyles.AllowHexSpecifier);
                BigInteger dec = BigInteger.ModPow(bint, Key.d, Key.n);
                numblock[i++] = (int)dec;
            }

            string plain = UtilityConverter.NumBlockToString(numblock);

            return plain;
        }
    }
}
