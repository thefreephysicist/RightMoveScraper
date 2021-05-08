using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace RightMove
{
	public class HttpHelper
	{
		/// <summary>
		/// Get a document from a url
		/// </summary>
		/// <param name="url">the url</param>
		/// <returns>Returns the <see cref="IDocument"/></returns>
		public static async Task<IDocument> GetDocument(string url, CancellationToken cancellationToken = default(CancellationToken))
		{
			var config = Configuration.Default.WithDefaultLoader();
			var context = BrowsingContext.New(config);
			var document = await context.OpenAsync(url, cancellationToken);
			return document;
		}
				
		public static byte[] DownloadRemoveImage(string uri, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (WebClient client = new WebClient())
			{
				// client.DownloadDataAsync(uri, cancellationToken);
				byte[] pic = client.DownloadData(uri);
				return pic;
			}
		}
	}
}
