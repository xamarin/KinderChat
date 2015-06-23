using System;
using System.Text.RegularExpressions;

namespace KinderChat
{
	public static class RegexUtilities
	{
		
		public static bool IsValidEmail(this string strIn)
		{
			if (String.IsNullOrEmpty(strIn))
				return false;

			// Return true if strIn is in valid e-mail format. 
			try {
				return Regex.IsMatch(strIn,
					@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
					@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
					RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
			}
			catch (RegexMatchTimeoutException) {
				return false;
			}
		}

		public static bool IsValidPhoneNumber(this string strIn)
		{
			if (String.IsNullOrEmpty(strIn))
				return false;

			// Return true if strIn is in valid e-mail format. 
			try {
				string pattern = @"
							^                  # From Beginning of line
							(?:\(?)            # Match but don't capture optional (
							(?<AreaCode>\d{3}) # 3 digit area code
							(?:[\).]?)         # Optional ) or .
							(?<Prefix>\d{3})   # Prefix
							(?:[-\.]?)         # optional - or .
							(?<Suffix>\d{4})   # Suffix
							(?!\d)             # Fail if eleventh number found";
											
				return Regex.IsMatch(strIn, pattern,RegexOptions.IgnorePatternWhitespace, TimeSpan.FromMilliseconds(250));
			}
			catch (RegexMatchTimeoutException) {
				return false;
			}
		}

		public static bool IsValidPin(this string strIn)
		{
			if (String.IsNullOrEmpty(strIn) || strIn.Length != 6)
				return false;

			// Return true if strIn is in valid e-mail format. 
			try {
				return Regex.IsMatch(strIn, @"^[0-9]+$",RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
			}
			catch (RegexMatchTimeoutException) {
				return false;
			}
		}
			
	}
}

