﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Newtonsoft.Json;

namespace RightMove
{
	public class RightMovePropertyPageParser
	{
		public RightMovePropertyPageParser(int propertyId)
		{
			PropertyId = propertyId;
		}

		public int PropertyId
		{
			get;
			private set;
		}
		
		public RightMoveProperty RightMoveProperty
		{
			get;
			private set;
		}
		
		public RightMovePropertyPage Page
		{
			get;
			private set;
		}
		
		public Rootobject Json
		{
			get;
			private set;
		}

		public async Task<bool> ParseRightMovePropertyPageAsync()
		{
			return await ParseRightMovePropertyPageAsync(PropertyId);
		}
		
		private async Task<bool> ParseRightMovePropertyPageAsync(int propertyId)
		{
			string url = RightMoveUrls.GetPropertyUrl(propertyId);
			var document = await HttpHelper.GetDocument(url);

			if (document is null)
			{
				return false;
			}
			
			ParseRightMovePropertyPage(document);
			return true;
		}

		private void ParseRightMovePropertyPage(IDocument document)
		{
			Json = GetJson(document);

			if (Json is null)
			{
				return;
			}
			
			RightMoveProperty property = new RightMoveProperty()
			{
				Address = $"{Json.propertyData.address.displayAddress}, {Json.propertyData.address.ukCountry}",
				Desc = Json.propertyData.text.description,
				Agent = Json.propertyData.customer.branchDisplayName,
			};

			property.Price = RightMoveParserHelper.ParsePrice(Json.propertyData.prices.primaryPrice);
			property.Date = RightMoveParserHelper.ParseDate(Json.propertyData.listingHistory.listingUpdateReason);
			property.ImageUrl = Json.propertyData.images.Select(o => o.url).ToArray();

			RightMoveProperty = property;
		}
		
		private static Rootobject GetJson(IDocument document)
		{
			var script = document.All.FirstOrDefault(o => o.LocalName.Equals("script") &&
			                                              o.Text().Trim().StartsWith("window.PAGE_MODEL"));

			if (string.IsNullOrEmpty(script?.Text()))
			{
				return null;
			}

			string start = "window.PAGE_MODEL = ";
			var indx = script?.Text().IndexOf(start);
			if (indx <= 0)
			{
				return null;
			}

			var jsonText = script.Text().Trim().Substring(start.Length);

			var json = JsonConvert.DeserializeObject<Rootobject>(jsonText);
			return json;
		}
	}
}
