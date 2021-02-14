using System;
using System.Collections.Generic;
using System.Text;

namespace RightMove
{
	public static class StringHelper
	{
		/// <summary>
		/// Trim a string of \r, \n and \t
		/// </summary>
		/// <param name="str">The <see cref="string"/> to trim</param>
		/// <returns>The trimmed string</returns>
		public static string TrimUp(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}

			var ret = str.Trim('\r', '\n', '\t');

			return ret.Trim();
		}
	}
}
