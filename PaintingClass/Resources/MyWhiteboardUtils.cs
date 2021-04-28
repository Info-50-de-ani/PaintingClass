using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintingClass.Resources
{
	public static class MyWhiteboardUtils
	{
        static int Val(char c)
        {
            if (c >= '0' && c <= '9')
                return (int)c - '0';
            else
                return (int)c - 'A' + 10;
        }

        static int ToDeci(string str,int b_ase)
        {
            int len = str.Length;
            int power = 1; 
            int num = 0; 
            int i;

            for (i = len - 1; i >= 0; i--)
            {
                num += Val(str[i]) * power;
                power = power * b_ase;
            }
            return num;
        }

        /// <summary>
        /// ia ARGB
        /// </summary>
        /// <param name="_hex"></param>
        /// <returns></returns>
        public static string InvertHex(string _hex)
		{
            string[] hex = { _hex.Substring(3, 2), _hex.Substring(5, 2), _hex.Substring(7, 2) };
            string res = "";
            foreach (var x in hex)
            {
                string s = Convert.ToString(255 - ToDeci(x, 16), 16);
                if (s.Length == 1)
                    res += "0" + s;
                else
                    res += s;
            }
            return "#" + res;
		}
    }
}
