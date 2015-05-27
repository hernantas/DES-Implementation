using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace DESEncryption
{
    public class UserCertificate
    {
        private int id = -1;
        public int Id
        {
            get { return id; }
        }

        public RSAKey publicKey = null;
        public string timestamp = "";

        public UserCertificate(int id)
        {
            this.id = id;
        }

        public override string ToString()
        {
            return timestamp + ";" + Id + ";" + publicKey.n + ";" + publicKey.e;
        }
    }
}
