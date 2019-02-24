using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxtDownLoader.utils.encrypt
{
    class HexUtil
    {
        public static void Test(string[] args) {
            string data1 = "dsjflajdflasjf12313213";
          
            Console.WriteLine("before1 value of {0}", data1);
            string data2 = StrToHex(data1);
            Console.WriteLine("before2 value of {0}", data2);

            string data3 = HexToStr(data2);
            Console.WriteLine("before3 value of {0}", data3);

        }
        public static string StrToHex(string input)
        {
            char[] values = input.ToCharArray();
            StringBuilder sb = new StringBuilder();
            foreach (char letter in values)
            {
                // Get the integral value of the character.
                int value = Convert.ToInt32(letter);
                // Convert the decimal value to a hexadecimal value in string form.
                string hexOutput = String.Format("{0:X}", value);
                sb.Append(hexOutput);

            }
            return sb.ToString();
        }

        public static string HexToStr(string mHex) // 返回十六进制代表的字符串
        {
            mHex = mHex.Replace(" ", "");
            if (mHex.Length <= 0) return "";
            byte[] vBytes = new byte[mHex.Length / 2];
            for (int i = 0; i < mHex.Length; i += 2)
                if (!byte.TryParse(mHex.Substring(i, 2), NumberStyles.HexNumber, null, out vBytes[i / 2]))
                    vBytes[i / 2] = 0;
            return ASCIIEncoding.Default.GetString(vBytes);
        } /* HexToStr */

        public static string  HexToStr2(string input)
        {
            string str = "";
            return str;

        }
    }
}
