using System;
using System.Collections;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESEncryption
{
    public class ECB
    {
        private static RSA rsa = new RSA();

        public static String encrypt(String plainText, string publicKey)
        {
            if (plainText == null)
                return "";

            ASCIIEncoding encoder = new ASCIIEncoding();
            DES des = new DES();

            byte[] kbytes = BigInteger.Parse(rsa.GeneratePrivateKey(publicKey)).ToByteArray();
            BitArray bitKey = new BitArray(kbytes);

            byte[] tempB = encoder.GetBytes(plainText);
            byte[] bytes = new byte[(int)Math.Ceiling(tempB.Length / 8.0f)*8];

            for (int i = 0; i < tempB.Length; i++)
            {
                bytes[i] = tempB[i];
            }

            BitArray[] group = new BitArray[(int)Math.Ceiling(bytes.Length/8.0)];

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < group.Length; i++)
            {
                byte[] b = new byte[8];

                for (int j = 0; j < 8; j++)
                {
                    b[j] = bytes[i * 8 + j];
                }

                group[i] = new BitArray(b);

                BitArray enc = des.encrypt(group[i], bitKey);

                //UtilityConverter.ShowBitsLine(enc);

                builder.Append(UtilityConverter.ToHex(enc));
            }

            return builder.ToString();
        }

        public static String decrypt(String chiperText, string publicKey)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            DES des = new DES();

            byte[] kbytes = BigInteger.Parse(rsa.GeneratePrivateKey(publicKey)).ToByteArray();
            BitArray bitKey = new BitArray(kbytes);

            BitArray bits = UtilityConverter.FromHex(chiperText);

            BitArray[] group = new BitArray[(int) (bits.Length/64)];

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < group.Length; i++)
            {
                group[i] = new BitArray(64);

                for (int j = 0; j < 64; j++)
                {
                    group[i][j] = bits[i * 64 + j];
                }

                BitArray dec = des.decrypt(group[i], bitKey);

                builder.Append(UtilityConverter.ToString(dec));
            }

            return builder.ToString().Replace("\0","");
        }
    }
}
