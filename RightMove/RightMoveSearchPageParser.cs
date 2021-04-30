using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace RightMove
{
	/// <summary>
	/// The selectors used for parsing
	/// </summary>


	public class RightMoveSearchPageParser
	{
		/// <summary>
		/// Class of selectors
		/// </summary>
		private static class Selector
		{
			public const string ResultsCount = "#searchHeader > span";
			public const string Link = "div > div > div.propertyCard-content > div.propertyCard-section > div.propertyCard-details > a";
			public const string HouseType = "div > div > div.propertyCard-content > div.propertyCard-section > div.propertyCard-details > a > h2";
			public const string Address = "div > div > div.propertyCard-content > div.propertyCard-section > div.propertyCard-details > a > address";
			public const string DateAndAgent = "div > div > div.propertyCard-content > div.propertyCard-detailsFooter > div.propertyCard-branchSummary";
			public const string Desc = "div > div > div.propertyCard-content > div.propertyCard-section > div.propertyCard-description";
			public const string Price = "div > div > div.propertyCard-header > div > a > div.propertyCard-priceValue";
			public const string Featured = "div > div > div.propertyCard-moreInfo > div.propertyCard-moreInfoFeaturedTitle";

			// these two page selectors will never work as this code is hidden behind a knockout JS 'component'
			public const string PageCount = "#l-container > div.l-propertySearch-paginationWrapper > div > div > div > div.pagination-pageSelect > span:nth-child(4)";
			public const string CurrentPage = "#l-container > div.l-propertySearch-paginationWrapper > div > div > div > div.pagination-pageSelect > div";
		}


		public RightMoveSearchPageParser(IDocument document)
		{
			if (document is null)
			{
				throw new ArgumentNullException(nameof(document));
			}

			ParseRightMoveSearchPage(document);
		}

		public RightMoveSearchPage Page
		{
			get;
			private set;
		}

		
		
		private void ParseRightMoveSearchPage(IDocument document)
		{
			var pageText = document.Body.QuerySelector(Selector.PageCount)?.Text();

			int page;
			if (!int.TryParse(pageText, out page))
			{
				page = -1;
			}

			var resultCountText = document.Body.QuerySelector(Selector.ResultsCount)?.Text();

			if (!int.TryParse(resultCountText, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out int resultsCount))
			{
				resultsCount = -1;
			}

			var currentPageText = document.QuerySelector(Selector.CurrentPage)?.Text();
			if (!int.TryParse(currentPageText, out int currentPage))
			{
				currentPage = -1;
			}

			var propertyNodes = document.All.Where(m => !string.IsNullOrEmpty(m.Id) && m.Id.StartsWith("property-"));

			RightMoveSearchItemCollection rightMoveItems = new RightMoveSearchItemCollection();

			foreach (var propertyNode in propertyNodes)
			{
				string propertyCardPrefix = "property-";
				int rightMoveId;
				if (!int.TryParse(propertyNode.Id.Substring(propertyCardPrefix.Length), out rightMoveId) || rightMoveId == 0)
				{
					continue;
				}

				// properties
				string houseType;
				string address;
				string desc;
				DateTime date = DateTime.MinValue;
				string agent = null;
				string link;
				int price;
				bool featured = false;

				houseType = propertyNode.QuerySelector(Selector.HouseType)?.Text();
				houseType = StringHelper.TrimUp(houseType);

				link = propertyNode.QuerySelector(Selector.Link).GetAttribute("href");
				address = propertyNode.QuerySelector(Selector.Address)?.Text();
				address = StringHelper.TrimUp(address);

				desc = propertyNode.QuerySelector(Selector.Desc)?.Text();
				desc = StringHelper.TrimUp(desc);

				var dateAndAgentText = propertyNode.QuerySelector(Selector.DateAndAgent)?.Text();

				// dateAndAgent is in the form "Added on 01/02/2021 by Melissa Berry Sales & Lettings, Prestwich"
				if (!string.IsNullOrEmpty(dateAndAgentText))
				{
					date = RightMoveParserHelper.ParseDate(dateAndAgentText);
					agent = RightMoveParserHelper.ParseAgent(dateAndAgentText);
				}

				var priceText = propertyNode.QuerySelector(Selector.Price)?.Text();

				price = RightMoveParserHelper.ParsePrice(priceText);

				var featuredText = propertyNode.QuerySelector(Selector.Featured)?.Text();
				if (featuredText?.IndexOf("featured", StringComparison.CurrentCultureIgnoreCase) >= 0)
				{
					featured = true;
				}

				RightMoveProperty rightMoveItem = new RightMoveProperty()
				{
					RightMoveId = rightMoveId,
					HouseInfo = houseType,
					Address = address,
					Desc = desc,
					Agent = agent,
					Date = date,
					Link = link,
					Price = price,
					Featured = featured
				};

				rightMoveItems.Add(rightMoveItem);
			}

			RightMoveSearchPage rightMovePage = new RightMoveSearchPage()
			{
				RightMoveSearchItems = new RightMoveSearchItemCollection(rightMoveItems),
				PageCount = page,
				CurrentPage = currentPage,
				ResultsCount = resultsCount
			};

			Page = rightMovePage;
		}
	}
}
