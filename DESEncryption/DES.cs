using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESEncryption
{
    public class DES
    {
        private int[] roundShift = { 1,	1,	2,	2,	2,	2,	2,	2,	1,	2,	2,	2,	2,	2,	2,	1  };
        private int[] permutationPos = {14, 17, 11, 24, 1, 5, 
                                        3, 28, 15, 6, 21, 10, 
                                        23, 19, 12, 4, 26, 8, 
                                        16, 7, 27, 20, 13, 2, 
                                        41, 52, 31, 37, 47, 55, 
                                        30, 40, 51, 45, 33, 48, 
                                        44, 49, 39, 56, 34, 53, 
                                        46, 42, 50, 36, 29, 32 };
        
        private int[] ipPos = { 58, 50, 42, 34, 26, 18, 10, 2,
                                60,	52,	44,	36,	28,	20,	12,	4,
                                62,	54,	46,	38,	30,	22,	14,	6,
                                64,	56,	48,	40,	32,	24,	16,	8,	
                                57,	49,	41,	33,	25,	17,	9,	1,
                                59,	51,	43,	35,	27,	19,	11,	3,
                                61,	53,	45,	37,	29,	21,	13,	5,	
	                            63,	55,	47,	39,	31,	23,	15,	 7 };
        private int[] ipPosInv = {  40, 8, 48, 16, 56, 24, 64, 32, 
                                    39, 7, 47, 15, 55, 23, 63, 31, 
                                    38, 6, 46, 14, 54, 22, 62, 30, 
                                    37, 5, 45, 13, 53, 21, 61, 29, 
                                    36, 4, 44, 12, 52, 20, 60, 28, 
                                    35, 3, 43, 11, 51, 19, 59, 27, 
                                    34, 2, 42, 10, 50, 18, 58, 26, 
                                    33, 1, 41, 9, 49, 17, 57, 25 };
        private int[] ipKeyPos = {  57, 49, 41, 33, 25, 17, 9, 
                                    1, 58, 50, 42, 34, 26, 18, 
                                    10, 2, 59, 51, 43, 35, 27, 
                                    19, 11, 3, 60, 52, 44, 36, 
                                    63, 55, 47, 39, 31, 23, 15, 
                                    7, 62, 54, 46, 38, 30, 22, 
                                    14, 6, 61, 53, 45, 37, 29, 
                                    21, 13, 5, 28, 20, 12, 4 };
        private int[] chiperPermutation = { 16,  7, 20, 21,
											29, 12, 28, 17,
											1, 15, 23, 26,
											5, 18, 31, 10,
											2,  8, 24, 14,
											32, 27,  3,  9,
											19, 13, 30,  6,
											22, 11,  4, 25};

        private BitArray bitReverse(BitArray bit)
        {
            BitArray newBit = new BitArray(bit.Length);

            for (int i = 0; i < bit.Length; i++)
            {
                newBit[i] = bit[bit.Length - i - 1];
            }

            return newBit;
        }

        public BitArray encrypt(BitArray bit64, BitArray bitKey64)
        {
            //bit64 = UtilityConverter.ToBit("0000000100100011010001010110011110001001101010111100110111101111");
            //bitKey64 = UtilityConverter.ToBit("0001001100110100010101110111100110011011101111001101111111110001");
            
            BitArray ip = InitialPermutation(bit64);
            BitArray[] ipHalf = SplitIP(ip);
            BitArray[] ipKey = KeyInitialPermutation(bitKey64);

            //UtilityConverter.ShowBitsLine(ip);

            /*foreach (BitArray bit in ipKey)
            {
                UtilityConverter.ShowBitsLine(bit);
            }
             * */

            for (int round = 0; round < 16; round++)
            {
                
                BitArray chiper = ChiperFunction(ipHalf[1], ipKey[round]);

                //UtilityConverter.ShowBits(ipHalf[0]);
                //Console.Write(" +");
                //UtilityConverter.ShowBits(chiper);
                //Console.Write(" = ");
                
                ipHalf[0] = ipHalf[0].Xor(chiper);

                //UtilityConverter.ShowBitsLine(ipHalf[0]);

                BitArray temp = ipHalf[0];
                ipHalf[0] = ipHalf[1];
                ipHalf[1] = temp;

                //UtilityConverter.ShowBits(ipHalf[0]);
                //UtilityConverter.ShowBitsLine(ipHalf[1]);
            }
            //Console.WriteLine();

            BitArray temps = ipHalf[0];
            ipHalf[0] = ipHalf[1];
            ipHalf[1] = temps;

            //UtilityConverter.ShowBits(ipHalf[0]);
            //UtilityConverter.ShowBitsLine(ipHalf[1]);

            BitArray reverse = reverseInitialPermutation(ipHalf);

            //UtilityConverter.ShowBitsLine(reverse);
            //Console.WriteLine();

            return reverse;
        }

        public BitArray decrypt(BitArray bit64, BitArray bitKey64)
        {
            //bit64 = UtilityConverter.ToBit("0000000100100011010001010110011110001001101010111100110111101111");
            //bitKey64 = UtilityConverter.ToBit("0001001100110100010101110111100110011011101111001101111111110001");

            BitArray ip = InitialPermutation(bit64);
            BitArray[] ipHalf = SplitIP(ip);
            BitArray[] ipKey = KeyInitialPermutation(bitKey64);

            Array.Reverse(ipKey);

            /*foreach (BitArray bit in ipKey)
            {
                UtilityConverter.ShowBitsLine(bit);
            }
             * */

            for (int round = 0; round < 16; round++)
            {

                BitArray chiper = ChiperFunction(ipHalf[1], ipKey[round]);

                //UtilityConverter.ShowBits(ipHalf[0]);
                //Console.Write(" +");
                //UtilityConverter.ShowBits(chiper);
                //Console.Write(" = ");

                ipHalf[0] = ipHalf[0].Xor(chiper);

                //UtilityConverter.ShowBitsLine(ipHalf[0]);

                BitArray temp = ipHalf[0];
                ipHalf[0] = ipHalf[1];
                ipHalf[1] = temp;

                //UtilityConverter.ShowBits(ipHalf[0]);
                //UtilityConverter.ShowBitsLine(ipHalf[1]);
            }
            //Console.WriteLine();

            BitArray temps = ipHalf[0];
            ipHalf[0] = ipHalf[1];
            ipHalf[1] = temps;

            //UtilityConverter.ShowBits(ipHalf[0]);
            //UtilityConverter.ShowBitsLine(ipHalf[1]);

            BitArray reverse = reverseInitialPermutation(ipHalf);

            //UtilityConverter.ShowBitsLine(reverse);
            //Console.WriteLine();

            return reverse;
        }

        private BitArray InitialPermutation(BitArray bit64)
        {
            BitArray ip = new BitArray(64);

            for (int i = 0; i < 64; i++)
            {
                ip[i] = bit64[(ipPos[i]-1)];
            }

            return ip;
        }

        private BitArray[] SplitIP(BitArray bit64)
        {
            BitArray[] bitHalf = new BitArray[2];
            bitHalf[0] = new BitArray(32);
            bitHalf[1] = new BitArray(32);

            for (int i = 0; i < 32; i++)
            {
                bitHalf[0][i] = bit64[i];
            }
            for (int i = 0; i < 32; i++)
            {
                bitHalf[1][i] = bit64[i+32];
            }

            return bitHalf;
        }

        private BitArray[] KeyInitialPermutation(BitArray bit64)
        {
            BitArray ip = new BitArray(56);

            for (int i = 0; i < 56; i++)
            {
                ip[i] = bit64[(ipKeyPos[i]-1)];
            }

            BitArray[] key = new BitArray[16];
            BitArray[] half = SplitKeyIP(ip);

            for (int i = 0; i < 16; i++)
            {
                half[0] = BinRotate(i, half[0]);
                half[1] = BinRotate(i, half[1]);
                key[i] = Permutation(half);
            }

            return key;
        }

        private BitArray[] SplitKeyIP(BitArray bit56)
        {
            BitArray[] bitHalf = new BitArray[2];
            bitHalf[0] = new BitArray(28);
            bitHalf[1] = new BitArray(28);

            for (int i = 0; i < 56; i++)
            {
                bitHalf[(int)(i / 28)][i%28] = bit56[i];
            }

            return bitHalf;
        }

        private BitArray BinRotate(int round, BitArray bitHalf28)
        {
            for (int shift = 0; shift < roundShift[round]; shift++)
            {
                bool firstBit = bitHalf28[0];
                for (int i = 1; i < bitHalf28.Length; i++)
                {
                    bitHalf28[i-1] = bitHalf28[i];
                }
                bitHalf28[bitHalf28.Length - 1] = firstBit;
            }

            return bitHalf28;
        }

        private BitArray[] BinRotateRight(int round, BitArray[] bitHalf28)
        {
            for (int shift = 0; shift < roundShift[round]; shift++)
            {
                bool lastBit = bitHalf28[0][bitHalf28[0].Length - 1];
                for (int i = 0; i < bitHalf28[0].Length-1; i++)
                {
                    bitHalf28[0][i] = bitHalf28[0][i+1];
                }
                bitHalf28[0][0] = lastBit;

                lastBit = bitHalf28[1][bitHalf28[1].Length - 1];
                for (int i = 0; i < bitHalf28[1].Length - 1; i++)
                {
                    bitHalf28[1][i] = bitHalf28[1][i + 1];
                }
                bitHalf28[1][0] = lastBit;
            }

            return bitHalf28;
        }

        private BitArray Combine(BitArray left, BitArray right)
        {
            BitArray combined = new BitArray(left.Length+right.Length);

            for (int i = 0; i < left.Length; i++)
            {
                combined[i] = left[i];
            }
            for (int i = 0; i < right.Length; i++)
            {
                combined[i + 28] = right[i];
            }

            return combined;
        }

        private BitArray Permutation(BitArray[] bitHalf28)
        {
            BitArray permutation = new BitArray(48);

            // Convert BitArray[] to BitArray
            BitArray bitHalfCombine = new BitArray(56);
            for (int i=0;i<28;i++)
            {
                bitHalfCombine[i] = bitHalf28[0][i];
            }
            for (int i=0;i<28;i++)
            {
                bitHalfCombine[i+28] = bitHalf28[1][i];
            }

            for (int i=0;i<48;i++)
            {
                permutation[i] = bitHalfCombine[(permutationPos[i]-1)];
            }

            return permutation;
        }

        private BitArray SBoxLookUp(int sboxChoice, int row, int col)
        {
            int[,] sbox =  { {14,  4, 13,  1,  2, 15, 11,  8,  3, 10,  6, 12,  5,  9,  0,  7,
							    0, 15,  7,  4, 14,  2, 13,  1, 10,  6, 12, 11,  9,  5,  3,  8,
							    4,  1, 14,  8, 13,  6,  2, 11, 15, 12,  9,  7,  3, 10,  5,  0,
							    15, 12,  8,  2,  4,  9,  1,  7,  5, 11,  3, 14, 10,  0,  6, 13 },
						    {15,  1,  8, 14,  6, 11,  3,  4,  9,  7,  2, 13, 12,  0,  5, 10,
							    3, 13,  4,  7, 15,  2,  8, 14, 12,  0,  1, 10,  6,  9, 11,  5,
							    0, 14,  7, 11, 10,  4, 13,  1,  5,  8, 12,  6,  9,  3,  2, 15,
							    13,  8, 10,  1,  3, 15,  4,  2, 11,  6,  7, 12,  0,  5, 14,  9},
						    {10,  0,  9, 14,  6,  3, 15,  5,  1, 13, 12,  7, 11,  4,  2,  8,
							    13,  7,  0,  9,  3,  4,  6, 10,  2,  8,  5, 14, 12, 11, 15,  1,
							    13,  6,  4,  9,  8, 15,  3,  0, 11,  1,  2, 12,  5, 10, 14,  7,
							    1, 10, 13,  0,  6,  9,  8,  7,  4, 15, 14,  3, 11,  5,  2, 12},
						    { 7, 13, 14,  3,  0,  6,  9, 10,  1,  2,  8,  5, 11, 12,  4, 15,
							    13,  8, 11,  5,  6, 15,  0,  3,  4,  7,  2, 12,  1, 10, 14,  9,
							    10,  6,  9,  0, 12, 11,  7, 13, 15,  1,  3, 14,  5,  2,  8,  4,
							    3, 15,  0,  6, 10,  1, 13,  8,  9,  4,  5, 11, 12,  7,  2, 14},
						    { 2, 12,  4,  1,  7, 10, 11,  6,  8,  5,  3, 15, 13,  0, 14,  9,
							    14, 11,  2, 12,  4,  7, 13,  1,  5,  0, 15, 10,  3,  9,  8,  6,
							    4,  2,  1, 11, 10, 13,  7,  8, 15,  9, 12,  5,  6,  3,  0, 14,
							    11,  8, 12,  7,  1, 14,  2, 13,  6, 15,  0,  9, 10,  4,  5,  3},
						    {12,  1, 10, 15,  9,  2,  6,  8,  0, 13,  3,  4, 14,  7,  5, 11,
							    10, 15,  4,  2,  7, 12,  9,  5,  6,  1, 13, 14,  0, 11,  3,  8,
							    9, 14, 15,  5,  2,  8, 12,  3,  7,  0,  4, 10,  1, 13, 11,  6,
							    4,  3,  2, 12,  9,  5, 15, 10, 11, 14,  1,  7,  6,  0,  8, 13},
						    { 4, 11,  2, 14, 15,  0,  8, 13,  3, 12,  9,  7,  5, 10,  6,  1,
							    13,  0, 11,  7,  4,  9,  1, 10, 14,  3,  5, 12,  2, 15,  8,  6,
							    1,  4, 11, 13, 12,  3,  7, 14, 10, 15,  6,  8,  0,  5,  9,  2,
							    6, 11, 13,  8,  1,  4, 10,  7,  9,  5,  0, 15, 14,  2,  3, 12},
						    {13,  2,  8,  4,  6, 15, 11,  1, 10,  9,  3, 14,  5,  0, 12,  7,
							    1, 15, 13,  8, 10,  3,  7,  4, 12,  5,  6, 11,  0, 14,  9,  2,
							    7, 11,  4,  1,  9, 12, 14,  2,  0,  6, 10, 13, 15,  3,  5,  8,
							    2,  1, 14,  7,  4, 10,  8, 13, 15, 12,  9,  0,  3,  5,  6, 11}};

            int c = sbox[sboxChoice, row * 16 + col];

            bool[] b = new bool[] { false, false, false, false };

            if (c == 0)
                b = new bool[] { false, false, false, false };
            else if (c == 1)
                b = new bool[] { false, false, false, true };
            else if (c == 2)
                b = new bool[] { false, false, true, false };
            else if (c == 3)
                b = new bool[] { false, false, true, true };
            else if (c == 4)
                b = new bool[] { false, true, false, false };
            else if (c == 5)
                b = new bool[] { false, true, false, true };
            else if (c == 6)
                b = new bool[] { false, true, true, false };
            else if (c == 7)
                b = new bool[] { false, true, true, true };
            else if (c == 8)
                b = new bool[] { true, false, false, false };
            else if (c == 9)
                b = new bool[] { true, false, false, true };
            else if (c == 10)
                b = new bool[] { true, false, true, false };
            else if (c == 11)
                b = new bool[] { true, false, true, true };
            else if (c == 12)
                b = new bool[] { true, true, false, false };
            else if (c == 13)
                b = new bool[] { true, true, false, true };
            else if (c == 14)
                b = new bool[] { true, true, true, false };
            else if (c == 15)
                b = new bool[] { true, true, true, true };

            return new BitArray(b);
        }

        private BitArray ChiperFunction(BitArray bit32, BitArray bit48)
        {
            // Expand
            int[] expnansion = {	32,  1,  2,  3,  4,  5,
								4,	 5,  6,  7,  8,  9,
								8,  9, 10, 11, 12, 13,
								12, 13, 14, 15, 16, 17,
								16, 17, 18, 19, 20, 21,
								20, 21, 22, 23, 24, 25,
								24, 25, 26, 27, 28, 29,
								28, 29, 30, 31, 32,  1};
            BitArray bitExp48 = new BitArray(48);

            for (int i = 0; i < 48; i++)
            {
                bitExp48[i] = bit32[(expnansion[i]-1)];
            }

            /*
            UtilityConverter.ShowBits(bit48);
            Console.Write(" | ");
            UtilityConverter.ShowBits(bit32);
            Console.Write(" >> ");
            UtilityConverter.ShowBitsLine(bitExp48);
             */

            // XOR
            bitExp48 = bitExp48.Xor(bit48);

            //UtilityConverter.ShowBitsLine(bitExp48);

            // convert to group
            BitArray[] group = new BitArray[8];
            for (int i = 0; i < 8; i++)
            {
                group[i] = new BitArray(6);

                for (int j=0;j<6;j++)
                {
                    group[i][j] = bitExp48[(i * 6) + j];
                }
            }
            
            BitArray sboxSubs = new BitArray(32);
            int subPos = 0;

            for (int i = 0; i < 8; i++)
            {
                bool[] firstLast =  { group[i].Get(0), group[i].Get(5) };
                BitArray row = new BitArray(firstLast);
                int rowInt = UtilityConverter.BinToInt(row);

                BitArray col = new BitArray(new bool[] 
                                    {group[i].Get(1), group[i].Get(2), group[i].Get(3), group[i].Get(4)});
                int colInt = UtilityConverter.BinToInt(col);

                //UtilityConverter.ShowBits(row);
                //Console.WriteLine(rowInt+", "+colInt);
                //UtilityConverter.ShowBitsLine(col);

                BitArray bit4 = SBoxLookUp(i, rowInt, colInt);

                //UtilityConverter.ShowBits(group[i]);
                //Console.Write(" >> ");
                //UtilityConverter.ShowBitsLine(bit4);

                for (int shift = 0; shift < 4; shift++)
                {
                    sboxSubs[subPos] = bit4[shift];
                    subPos++;
                }
            }

            

            BitArray perm = new BitArray(32);
            for (int i = 0; i < 32; i++)
            {
                perm[i] = sboxSubs[(chiperPermutation[i]-1)];
            }
            
            //UtilityConverter.ShowBits(sboxSubs);
            //Console.Write(" >> ");
            //UtilityConverter.ShowBitsLine(perm);

            return perm;
        }

        private BitArray reverseInitialPermutation(BitArray[] bit32)
        {
            BitArray bit64 = new BitArray(64);

            for (int i = 0; i < 64; i++)
            {
                bit64[i] = bit32[i/32][i%32];
            }

            BitArray ipInvers = new BitArray(64);
            for (int i = 0; i < 64; i++)
            {
                ipInvers[i] = bit64[ipPosInv[i]-1];
            }

            return ipInvers;
        }
    }
}
