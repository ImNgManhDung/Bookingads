using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookingAds.Modules
{
    public static class ExtralNumberUtils
    {
        public static int[] ConvertStringToNumbers(string numberString)
        {
            List<int> numbers = new List<int>();
            foreach (char c in numberString)
            {
                if (char.IsDigit(c))
                {
                    numbers.Add(int.Parse(c.ToString()));
                }
            }

            return numbers.ToArray();
        }

        public static string[] ConvertNumbersToWords(int[] numbers)
        {
            string[] words = new string[numbers.Length];
            for (int i = 0; i < numbers.Length; i++)
            {
                words[i] = ConvertNumberToWord(numbers[i]);
            }

            return words;
        }

        public static string ConvertNumberToWord(int number)
        {
            switch (number)
            {
                case 0: return "Không";
                case 1: return "Một";
                case 2: return "Hai";
                case 3: return "Ba";
                case 4: return "Bốn";
                case 5: return "Năm";
                case 6: return "Sáu";
                case 7: return "Bảy";
                case 8: return "Tám";
                case 9: return "Chín";
                default: return string.Empty;
            }
        }

        public static string ConvertArrayToString(string[] array)
        {
            return string.Join(", ", array);
        }
    }
}