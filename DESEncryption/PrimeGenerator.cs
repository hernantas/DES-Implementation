using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESEncryption
{
    public class PrimeGenerator
    {
        private Random rndGen = new Random();

        public PrimeGenerator()
        {
        }

        public void GeneratePrimeKey()
        {
        }

        private ulong GetRandomNumber()
        {
            ulong rndA = 0UL;

            for (int i = 0; i < 15; i++)
            {
                rndA = (rndA * 10UL) + Convert.ToUInt64(rndGen.Next(0, 10));
            }

            return rndA;
        }

        private ulong GCD(ulong a, ulong b)
        {
            ulong temp = 0;

            while (b != 0)
            {
                temp = a % b;
                a = b;
                b = temp;
            }
            return a;
        }
    }
}