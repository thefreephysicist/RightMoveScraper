using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace RightMove
{
	public class RightMoveParser
	{
		/// <summary>
		/// Initializes a new instance <see cref="RightMoveParser"/> class
		/// </summary>
		/// <param name="searchParams">the <see cref="SearchParams"></param>
		public RightMoveParser(SearchParams searchParams)
		{
			SearchParams = searchParams ?? throw new ArgumentNullException(nameof(searchParams));
		}

		/// <summary>
		/// Gets the <see cref="SearchParams"/>
		/// </summary>
		public SearchParams SearchParams
		{
			get;
		}

		/// <summary>
		/// Gets the list of results
		/// </summary>
		public RightMoveSearchItemCollection Results
		{
			get;
			private set;
		}

		/// <summary>
		/// Perform a search
		/// </summary>
		/// <returns>true if successful, false otherwise</returns>
		public async Task<bool> SearchAsync()
		{
			string searchUrl = $"{RightMoveUrls.SearchUrl}?{SearchParams.EncodeOptions()}";

			// the multiple for the page number
			int multiple = 24;

			int currentPage = 1;
			int pageCount = 1;

			RightMoveSearchItemCollection rightMoveItems = new RightMoveSearchItemCollection();
			
			// loop through all the pages
			while (currentPage <= pageCount)
			{
				int index = (currentPage - 1) * multiple;
				var searchUrlWithPageNumber = searchUrl + "&index=" + index;

				// parse the search page
				RightMoveSearchPage rightMovePage = await ParseRightMoveSearchPageAsync(searchUrlWithPageNumber);

				if (rightMovePage is null)
				{
					return false;
				}
				if (rightMovePage.RightMoveSearchItems is null)
				{
					break;
				}

				if (rightMovePage.ResultsCount >= 0 && pageCount == 1)
				{
					pageCount = (int)Math.Ceiling(rightMovePage.ResultsCount / 24.0);
				}

				rightMoveItems.AddRangeUnique(rightMovePage.RightMoveSearchItems);
				
				// increment the page number
				currentPage++;
			}

			System.Diagnostics.Debug.WriteLine("Finished searching pages");

			Results = rightMoveItems;

			return true;
		}

		/// <summary>
		/// ParseDate a right move search page
		/// </summary>
		/// <param name="searchUrl"></param>
		/// <returns>returns <see cref="RightMoveSearchPage"/> is successful, or null otherwise</returns>
		private async Task<RightMoveSearchPage> ParseRightMoveSearchPageAsync(string searchUrl)
		{
			var document = await HttpHelper.GetDocument(searchUrl);

			if (document is null)
			{
				return null;
			}

			RightMoveSearchPageParser searchPageParser = new RightMoveSearchPageParser(document);
			return searchPageParser.Page;
		}
	}
}
