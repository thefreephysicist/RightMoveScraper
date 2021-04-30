using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RightMove;

namespace RightMoveConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			/*
			RightMoveRegionScraper s = new RightMoveRegionScraper();
			Task.Run(async () => await s.DoSearchAsync()).Wait();
			*/

			string json = File.ReadAllText(@"C:\Users\maatb\source\repos\RightMoveScraper\RightMoveConsole\bin\Release\netcoreapp3.1\regions.json");
			var lst = JsonConvert.DeserializeObject(json);
			
			Console.WriteLine("Hello World!");
		}
	}
}
