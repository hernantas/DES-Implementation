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
            return generator.GetPrime().ToString();
        }
    }
}
