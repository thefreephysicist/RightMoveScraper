using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using RightMove;

namespace RightMoveTests
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Test1()
		{
			Assert.Pass();
		}
		
		[Test]
		public void ParseSearchPage_Null()
		{
			Assert.That(() => new RightMoveParser(null), Throws.ArgumentNullException);
		}
		
		[Test]
		public async Task ParseSearchPage()
		{
			SearchParams searchParams = new SearchParams()
			{
				MinBedrooms = 2,
				MaxBedrooms = 3,
				MinPrice = 100000,
				MaxPrice = 300000,
				Sort = SortType.HighestPrice,
				Radius = 0
			};

			RightMoveParser rightMoveParser = new RightMoveParser(searchParams);
			var searchResults = await rightMoveParser.SearchAsync();
			Debug.Assert(searchResults);
			Debug.Assert(rightMoveParser.Results.Count > 0);
		}

		[Test]
		public async Task ParsePropertyPage()
		{
			SearchParams searchParams = new SearchParams()
			{
				MinBedrooms = 2,
				MaxBedrooms = 3,
				MinPrice = 100000,
				MaxPrice = 300000,
				Sort = SortType.HighestPrice,
				Radius = 0
			};

			RightMovePropertyPageParser parser = new RightMovePropertyPageParser(90580477);
			await parser.ParseRightMovePropretyPageAsync();
		}

		[Test]
		public void ParsePage()
		{
			
		}
	}
}