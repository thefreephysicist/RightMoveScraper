using System.Collections.Generic;
using Newtonsoft.Json;

namespace RightMove
{
	public class RightMoveCodes
	{
		/// <summary>
		/// Gets a dictionary of the outcodes
		/// </summary>
		public static Dictionary<string, int> OutcodeDictionary
		{
			get;
		}

		static RightMoveCodes()
		{
			var outcodeJson = JsonConvert.DeserializeObject<dynamic>(Properties.Resources.Outcodes);

			OutcodeDictionary = new Dictionary<string, int>();
			foreach (var g in outcodeJson)
			{
				OutcodeDictionary.Add((string)g.outcode, (int)g.code);
			}
		}
	}
}
