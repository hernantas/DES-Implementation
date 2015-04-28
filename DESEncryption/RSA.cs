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

        public string GenerateKey()
        {
            ulong first = generator.GetPrime();
            ulong second = generator.GetPrime();
            ulong mult = (first * second);
            ulong e =  (first - 1) * (second -1);
            ulong coprime = generator.GetCoPrime(e);

            Console.WriteLine("First: " + first.ToString() + " Second: " + second.ToString() +
                " Multiplier: " + mult + " E: " + e + " CoPrime: " + coprime);

            return "First: " + first.ToString() + " Second: " + second.ToString() +
                " Multiplier: " + mult + " E: " + e + " CoPrime: " + coprime;
        }
    }
}
