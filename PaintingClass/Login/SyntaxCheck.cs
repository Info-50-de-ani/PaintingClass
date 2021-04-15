using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PaintingClass.Login
{
	static class SyntaxCheck
	{
		private static Regex regexName = new Regex(@"^[a-zA-Z ]{4,20}$");
		private static Regex regexEmail = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
		private static Regex regexPassword = new Regex(@"[\w\d\@$!%*#?&]{8,20}");

		public static bool CheckEmail(string _string)
		{
			return regexEmail.IsMatch(_string);
		}

		public static bool CheckName(string _string)
		{
			return regexName.IsMatch(_string);
		}

		public static bool CheckPassword(string _string)
		{
			return regexPassword.IsMatch(_string);
		}
	}
}
