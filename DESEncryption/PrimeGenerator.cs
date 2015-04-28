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
        private List<ulong> firstPrime = new List<ulong>();

        public PrimeGenerator()
        {
            PreFetchPrime();
        }

        private void PreFetchPrime()
        {
            List<ulong> firstNumber = new List<ulong>();

            for (ulong i = 0UL; i < Math.Sqrt(999999999999999); i++)
            {
                firstNumber.Add(i);
            }

            firstNumber[0] = 0;
            firstNumber[1] = 0;

            for (int i = 2; i < firstNumber.Count/2; i++)
            {
                if (firstNumber[i] != 0UL)
                {
                    for (int j = (int)firstNumber[i] * 2; j < firstNumber.Count; j += (int)firstNumber[i])
                    {
                        firstNumber[j] = 0;
                    }
                }
            }

            for (int i = 2; i < firstNumber.Count; i++)
            {
                if (firstNumber[i] != 0UL)
                {
                    firstPrime.Add(firstNumber[i]);
                }
            }
        }

        public ulong GetPrime()
        {
            ulong randNum = GetRandomNumber();

            while (!IsPrime(randNum))
            {
                randNum = GetRandomNumber();
            }

            return randNum;
        }

        private bool IsPrime(ulong num)
        {
            if (num <= 3)
                return false;

            for (int i = 0; firstPrime[i] <= Math.Sqrt(num); i++)
            {
                if (num % firstPrime[i] == 0)
                {
                    return false;
                }

                // Console.WriteLine("Pass: " + num + " % " + firstPrime[i] + " = " + (num % firstPrime[i]));
            }

            Console.WriteLine(num + " is prime");

            return true;
        }

        private ulong GetRandomNumber()
        {
            ulong rndA = 0UL;

            for (int i = 0; i < 15; i++)
            {
                if (i==0)
                    rndA = Convert.ToUInt64(rndGen.Next(1, 10));
                else
                    rndA = (rndA * 10UL) + Convert.ToUInt64(rndGen.Next(0, 10));
            }

            return rndA;
        }

        public ulong GetCoPrime(ulong num)
        {
            for (int i = firstPrime.Count-1; i >= 0; i--)
            {
                if (GreatestCommondDivisor(num, firstPrime[i]) == 1UL)
                {
                    return firstPrime[i];
                }
            }

            return 0;
        }

        public ulong GreatestCommondDivisor(ulong a, ulong b)
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