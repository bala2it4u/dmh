using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        public Program(string data) { }

        public void temp()
        {

        }
        static void Main(string[] args)
        {
            /*
                input given : 13(2)13(2)10#
                output array: {1,3,3,1,3,3,10}
                input given : "12(3)26#126#(2)
                output array: {1,2,2,2,26,1,26,26}
             */

            string input = "12(3)26#126#(2)";
            int[] data =  ResalveString(input);
            
            if ("1,2,2,2,26,1,26,26" == string.Join(",", data))
            {
                input = "13(2)13(2)10#";
                data = ResalveString(input);
                if ("1,3,3,1,3,3,10" == string.Join(",", data))
                {

                }
            }


            Missing();

            int ret = remAnagram("bcadeh", "hea");

            isUniqueChars("andaan");

            duplicateNumber();

            return;

            Console.WriteLine("test");
            int temp = FindMaxSum(new List<int> { 5, 9, 7, 11 });
            int temp1 = FindMaxSumSimple(new List<int> { 17, 19, 7, 11 });


            temp = CountCandies1(3, 2);
            temp = CountCandies1(710, 6);
            temp = CountCandies1(510, 6);
            temp = CountCandies1(910, 6);
            temp = CountCandies1(100, 5);
            temp = CountCandies1(150, 7);
            temp = CountCandies1(190, 8);
            temp = CountCandies1(101, 4);
            temp = CountCandies1(150, 2);
            temp = CountCandies1(170, 9);
            temp = CountCandies1(100, 13);

            Console.ReadKey();
        }

        static int[] ResalveString(string input)
        {
            List<int> output = new List<int>();
            int brackC = -1;
            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '(':
                        brackC = 0;

                        break;
                    case ')':
                        while (brackC != 1)
                        {
                            output.Add(output[output.Count - 1]);
                            brackC--;
                        }
                        brackC = -1;
                        break;

                    case '#':
                        output[output.Count - 2] = int.Parse($"{output[output.Count - 2]}{output[output.Count - 1]}");
                        output.RemoveAt(output.Count - 1);
                        break;

                    //case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8': case '9': case '0':
                    default:
                        if (brackC >= 0)
                            brackC = int.Parse($"{brackC}{input[i]}");
                        else
                            output.Add(int.Parse(input[i].ToString()));

                        break;
                }
                continue;
                
             //   return output.ToArray();

                if (int.TryParse(input[i].ToString(), out int value))
                {
                    if (brackC >= 0)
                    {
                        brackC = int.Parse($"{brackC}{value}");
                    }
                    else
                    {
                        output.Add(value);
                    }
                }
                else if (input[i] == '(')
                {
                    brackC = 0;
                }
                else if (input[i] == ')')
                {
                    while (brackC != 1)
                    {
                        output.Add(output[output.Count - 1]);
                        brackC--;
                    }
                    brackC = -1;
                }
                else if (input[i] == '#')
                {
                    output[output.Count - 2] = int.Parse($"{output[output.Count - 2]}{output[output.Count - 1]}");
                    output.RemoveAt(output.Count - 1);
                }
            }
            return output.ToArray();
        }

        static int Missing()
        {
            string str1 = "tea";// cde";
            string str2 = "toe";

            //getline(cin, str1);
            //getline(cin, str2);

            int[] A = new int[26];
            int[] B = new int[26];
            int[] output = new int[str1.Length];

            int i;

            for (i = 0; i < str1.Length; i++)
                A[str1[i] - 'a']++;

            for (i = 0; i < str2.Length; i++)
                B[str2[i] - 'a']++;

            for (i = 0; i < output.Length; i++)
            {
                output[i] = Math.Abs(A[str2[i] - 'a'] - B[str2[i] - 'a']);
            }

            int outp = 0;
            for (i = 0; i < 26; i++)
            {
                outp = outp + A[i] + B[i] - 2 * Math.Min(A[i], B[i]);
            }

            Console.WriteLine(outp);

            return 0;
        }

        static int remAnagram(String str1, String str2)
        {
            // make hash array for both string 
            // and calculate frequency of each
            // character
            int[] count1 = new int[26];
            int[] count2 = new int[26];

            System.Threading.Tasks.Parallel.For(0, str1.Length, (i, op) => { count1[str1[i] - 'a']++; });
            System.Threading.Tasks.Parallel.For(0, str2.Length, (i) => { count1[str1[i] - 'a']++; });
            // count frequency of each charcter 
            // in first string
            //for (int i = 0; i < str1.Length; i++)
            //    count1[str1[i] - 'a']++;

            //// count frequency of each charcter 
            //// in second string
            //for (int i = 0; i < str2.Length; i++)
            //    count2[str2[i] - 'a']++;

            // traverse count arrays to find 
            // number of charcters to be removed

            int result = 0;
            //System.Threading.Tasks.Parallel.For(0, 26, (i) => {
            //    if (Math.Abs(count1[i] - count2[i]) > 0)
            //        System.Threading.Interlocked.Increment(ref result);
            //});

            for (int i = 0; i < 26; i++)
                result += Math.Abs(count1[i] -
                                   count2[i]);
            return result;
        }

        public int methos(string data)
        {


            //static void Main(String[] args)
            //{
            //    int q = Convert.ToInt32(Console.ReadLine());
            //    for (int a0 = 0; a0 < q; a0++)
            //    {
            //        string s = Console.ReadLine();
            //        int result = anagaram(s);
            //        Console.WriteLine(result);
            //    }

            //}

            //static int anagaram(string data)
            //{

            int[] A = new int[26];
            int[] B = new int[26];
            int checkerA = 0;
            int checkerB = 0;

            //int[] output = new int[str1.Length];
            if (data.Length % 2 != 0)
                return -1;

            int holf = data.Length / 2;
            int i = 0;

            //for (i = 0; i < str1.Length; i++)
            //  A[str1[i] - 'a']++;

            for (i = 0; i < data.Length; i++)
            {
                int val = data[i] - 'a';
                if (holf > i)
                {
                    A[val]++;
                    checkerA |= (1 << val);
                    //A[data[i] - 'a']++;
                }
                else
                {
                    B[val]++;
                    checkerB |= (1 << val);

                    //if ((checker & (1 << val)) > 0) return false;


                }
            }


            int outA = 0;
            int outB = 0;
            for (i = 0; i < 26; i++)
            {
                // if (data == "xaxbbbxx")
                //    Console.WriteLine("A="+A[i] +"B="+B[i]);

                if (A[i] == 0 && B[i] != 0)
                {
                    outA += B[i];
                }
                else if (A[i] != 0 && B[i] < A[i])// && B[i] == 0)
                {
                    outB += A[i] - B[i];
                }
                //Console.WriteLine("A0="+(checkerA & (1 << i)));
                if ((checkerA & (1 << i)) > 0 && (checkerB & (1 << i)) == 0)
                {
                    //  outB++;
                }
                // else if (A[i] != 0 && B[i] < A[i])
                //  {   
                //     outB+= Math.Abs(A[i]- B[i]);
                // }
                //outp += Math.Abs(A[data[i] -'a'] - B[data[i] - 'a']); //
                //outp += A[i] + B[i] - 2 * Math.Min(A[i], B[i]);
            }

            return outB;//Math.Max(outA, outB);




        }

        public static bool isUniqueChars(String str)
        {
            int checker = 0;
            for (int i = 0; i < str.Length; i++)
            {
                int val = str[i] - 'a';
                if ((checker & (1 << val)) > 0)
                {
                    return false;
                }
                checker |= (1 << val);
            }
            return true;
        }

        public bool isFallls(string data)
        {
            int checker = 0;

            for (int i = 0; i < data.Length; i++)
            {
                int d = data[i] - 'a';
                if ((checker & i << d) > 0) { return false; }
                checker |= i << d;
            }

            return true;
        }

        private static bool duplicateNumber()
        {
            List<int> array = new List<int> { 1, 6, 90, 1, 90, 6, 6 };
            Dictionary<int, int> dic = new Dictionary<int, int>();
            int i = 0;
            //var foundAlready = new bool[array.Length];
            while (array.Count > i)
            {
                int temp = Math.Abs(array[i++]) - 1;
                if (array.Count - 1 < temp)
                {
                    if (dic.ContainsKey(temp) == false)
                        dic.Add(temp, temp);
                    else
                        Console.WriteLine(temp + 1);

                }
                else if (array[temp] >= 0)
                    array[temp] = -array[temp];
                else
                    Console.WriteLine(temp + 1);

            }
            return false;
        }

        public static int CountRecursive(int startingAmount, int newEvery, int total = 0)
        {
            if (startingAmount < newEvery)
                return ++total + startingAmount;

            total++;
            int extra = total % newEvery == 0 ? 1 : 0;
            return CountRecursive(startingAmount + extra - 1, newEvery, total);
        }

        public static int CountRecursiveless(int startingAmount, int newEvery, int total = 0)
        {
            if (startingAmount < newEvery)
                return total + startingAmount;

            total += newEvery;
            //int extra = total % newEvery == 0 ? 1 : 0;
            return CountRecursiveless(startingAmount - newEvery + 1, newEvery, total);
        }

        public static int CountCandies1(int startingAmount, int newEvery)
        {
            //int totalCandyEaten = 0;

            int candyEatenSplit = startingAmount / newEvery;
            int candyEatenSplitTotal = candyEatenSplit + (startingAmount % newEvery);
            while (candyEatenSplit > 1)
            {
                candyEatenSplit = candyEatenSplit / newEvery;
                candyEatenSplitTotal += candyEatenSplit;
            }
            int extra = (candyEatenSplitTotal) % newEvery;
            //int extra1 = candyEatenSplit % newEvery;

            int temp = startingAmount + candyEatenSplitTotal + extra;

            //int temp = startingAmount + candyEatenSplit +(extra + candyEatenSplit % newEvery )/ newEvery);

            //nt  temp;
            int totalCandyEaten = 0;
            //candyEatenSplit = 0;
            int i = 0;
            for (i = startingAmount; i >= newEvery; i -= (newEvery - 1))
            {
                totalCandyEaten += newEvery;
            }

            //extra = startingAmount % newEvery;
            totalCandyEaten += i;

            bool result = totalCandyEaten == temp;// == CountRecursiveless(startingAmount, newEvery);

            if (result == false)
            {
                throw new Exception();
            }
            return totalCandyEaten;
            //return totalCandyEaten;
        }

        public static int CountCandies(int startingAmount, int newEvery)
        {
            int totalCandyEaten = 0;
            int candyEatenSplit = 0;

            for (int i = startingAmount - 1; i >= 0; i--)
            {
                totalCandyEaten++;
                candyEatenSplit++;
                if (candyEatenSplit % newEvery == 0)
                {
                    i++;
                    candyEatenSplit = 0;
                }
            }

            return totalCandyEaten;
        }

        public static int FindMaxSumSimple(List<int> list)
        {
            if (list.Count <= 2)
                return list[0] + (list.Count > 1 ? list[1] : 0);
            int max1 = list[0];
            int max2 = list[1];
            if (max1 < max2)
            {
                var temp = max2;
                max2 = max1;
                max1 = temp;
            }

            for (int a = 2; a < list.Count; a++)
            {
                if (max1 < list[a])
                {
                    max2 = max1;
                    max1 = list[a];
                }
                else if (max2 < list[a])
                {
                    max2 = list[a];
                }
            }
            return max1 + max2;
        }

        public static int FindMaxSum(List<int> list)
        {
            int max = 0;
            for (int a = 0; a < list.Count; a++)
            {
                for (int b = 0; b < list.Count; b++)
                {
                    if (a == b)
                        continue;

                    var temp = list[a] + list[b];
                    if (temp > max)
                    {
                        max = temp;
                    }
                }
            }
            return max;
        }

    }
}
