using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESEncryption
{
    public class UtilityConverter
    {
        public static byte ToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }

            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }

        public static int BinToInt(BitArray bit)
        {
            int value = 0;

            for (int i = 0; i < bit.Length; i++)
            {
                if (bit[(bit.Length-1)-i])
                    value += Convert.ToInt16(Math.Pow(2, i));
            }

            return value;
        }

        public static String ToString(BitArray bits)
        {
            StringBuilder str = new StringBuilder();
            ASCIIEncoding encoder = new ASCIIEncoding();

            for (int i = 0; i < bits.Length; i += 8)
            {
                BitArray bit8 = new BitArray(8);

                for (int j = 0; j < 8; j++)
                {
                    bit8[j] = bits[i + j];
                }

                byte b = ToByte(bit8);
                char c = Convert.ToChar(b);
                str.Append(c);
            }

            return str.ToString();
        }

        public static String ToHex(BitArray bits)
        {
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < bits.Length; i += 8)
            {
                BitArray bit8 = new BitArray(8);

                for (int j = 0; j < 8; j++)
                {
                    bit8[j] = bits[i + j];
                }

                byte[] data = new byte[] { ToByte(bit8) };

                str.Append(BitConverter.ToString(data));
            }

            return str.ToString();
        }

        public static BitArray FromHex(String hex)
        {
            BitArray bits = new BitArray((hex.Length/2)*8);
            int bitPos = 0;

            for (int i = 0; i < hex.Length; i+=2)
            {
                String ffHex = hex[i] + "" + hex[i+1];
                byte b;

                try
                {
                    b = Convert.ToByte(ffHex, 16);
                }
                catch (Exception ex)
                {
                    b = new byte();
                }

                BitArray arr = new BitArray(new byte[] { b });

                for (int j = 0; j < arr.Length; j++)
                {
                    bits[bitPos] = arr[j];
                    bitPos++;
                }
            }

            return bits;
        }

        public static string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }

        public static BitArray HexToBit(char c)
        {
            bool[] b = new bool[4];

            switch (c)
            {
                case '0':
                    b = new bool[] { false, false, false, false }; break;
                case '1':
                    b = new bool[] { false, false, false, true };break;
                case '2':
                    b = new bool[] { false, false, true, false };break;
                case '3':
                    b = new bool[] { false, false, true, true };break;
                case '4':
                    b = new bool[] { false, true, false, false };break;
                case '5':
                    b = new bool[] { false, true, false, true };break;
                case '6':
                    b = new bool[] { false, true, true, false };break;
                case '7':
                    b = new bool[] { false, true, true, true };break;
                case '8':
                    b = new bool[] { true, false, false, false };break;
                case '9':
                    b = new bool[] { true, false, false, true };break;
                case 'a':
                case 'A':
                    b = new bool[] { true, false, true, false };break;
                case 'b':
                case 'B':
                    b = new bool[] { true, false, true, true };break;
                case 'c':
                case 'C':
                    b = new bool[] { true, true, false, false };break;
                case 'd':
                case 'D':
                    b = new bool[] { true, true, false, true };break;
                case 'e':
                case 'E':
                    b = new bool[] { true, true, true, false };break;
                case 'f':
                case 'F':
                    b = new bool[] { true, true, true, true };break;
                default:
                    throw new FormatException("Unrecognized hex char " + c);
            }

            return new BitArray(b);
        }

        public static void ShowBits(BitArray bits)
        {
            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                    Console.Write("1");
                else
                    Console.Write("0");

                if ((i+1) % 4 == 0 && i != 0)
                    Console.Write(" ");
            }
        }

        public static BitArray ToBit(string bin)
        {
            BitArray bit = new BitArray(bin.Length);

            for (int i = 0; i < bin.Length; i++)
            {
                if (bin[i] == '1')
                    bit[i] = true;
                else
                    bit[i] = false;
            }

            return bit;
        }

        public static void ShowBitsLine(BitArray bits)
        {
            ShowBits(bits);
            Console.WriteLine();
        }
    }
}
