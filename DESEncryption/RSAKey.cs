using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DESEncryption
{
    public class RSAKey
    {
        public BigInteger n { set; get; }
        public BigInteger e { set; get; }
        public BigInteger d { set; get; }

        public RSAKey()
        {
        }

        public RSAKey(BigInteger n, BigInteger e)
        {
            this.n = n;
            this.e = e;
        }
    }
}
