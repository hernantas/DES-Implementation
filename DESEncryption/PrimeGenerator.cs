using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DESEncryption
{
    public class PrimeGenerator
    {
        public BigInteger FindPrime()
        {
            BigInteger x = this.GetRandomNumber(15);

            for (int i = 0; i < 10 * BigInteger.Log(x) + 3; i++)
            {
                if (this.MillerRabin(x, 50))
                    return x;
                else
                    x++;
            }

            return -1;
        }

        public BigInteger FindCoPrime(BigInteger prime)
        {
            for (BigInteger e = 3; e < prime-1; e++)
            {
                if (BigInteger.GreatestCommonDivisor(prime, e) == 1)
                {
                    return e;
                }
            }

            return -1;
        }

        private BigInteger GetRandomNumber(int length)
        {
            Random random = new Random();
            BigInteger ret = new BigInteger();
            for (int i = 0; i < length; i++)
            {
                if (i == 0)
                {
                    ret = random.Next(1, 10);
                }
                else
                {
                    ret = (ret * 10) + random.Next(0, 10);
                }
            }

            return ret;
        }

        private bool MillerRabin(BigInteger n, int certainty)
        {
            if (n == 2 || n == 3)
                return true;

            if (n < 2 || n % 2 == 0)
                return false;

            BigInteger d = n - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[n.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < certainty; i++)
            {
                do
                {
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= n - 2);

                BigInteger x = BigInteger.ModPow(a, d, n);
                if (x == 1 || x == n - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == 1)
                        return false;
                    if (x == n - 1)
                        break;
                }

                if (x != n - 1)
                    return false;
            }

            return true;
        }
    }
}