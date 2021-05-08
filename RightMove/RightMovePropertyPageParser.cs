using System;
using AngleSharp.Dom;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

		public async Task<bool> ParseRightMovePropertyPageAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return await ParseRightMovePropertyPageAsync(PropertyId, cancellationToken);
		}

		private async Task<bool> ParseRightMovePropertyPageAsync(int propertyId, CancellationToken cancellationToken = default(CancellationToken))
		{
			string url = RightMoveUrls.GetPropertyUrl(propertyId);
			IDocument document = await HttpHelper.GetDocument(url, cancellationToken);

			if (document is null)
			{
				return false;
			}

			if (cancellationToken.IsCancellationRequested)
			{
				cancellationToken.ThrowIfCancellationRequested();
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

			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				MissingMemberHandling = MissingMemberHandling.Ignore
			};
			
			var json = JsonConvert.DeserializeObject<Rootobject>(jsonText, settings);
			return json;
		}
	}
}
