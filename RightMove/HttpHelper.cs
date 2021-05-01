using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
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
		public static async Task<IDocument> GetDocument(string url)
		{
			var config = Configuration.Default.WithDefaultLoader();
			var context = BrowsingContext.New(config);
			var document = await context.OpenAsync(url);
			return document;
		}
				
		public static byte[] DownloadRemoveImage(string uri)
		{
			using (WebClient client = new WebClient())
			{
				byte[] pic = client.DownloadData(uri);
				return pic;
			}
		}
		/*
		public static void DownloadRemoteImageFile(string uri, string filename)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			// check remote file exists
			if ((response.StatusCode == HttpStatusCode.OK ||
			 response.StatusCode == HttpStatusCode.Moved ||
			 response.StatusCode == HttpStatusCode.Redirect) &&
				response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
			{
				// if the remote file was found, download oit
				using (Stream inputStream = response.GetResponseStream())
				{
					byte[] buffer = new byte[4096];
					int bytesRead;
					do
					{
						bytesRead = inputStream.Read(buffer, 0, buffer.Length);
					} while (bytesRead != 0);
				}

				Bitmap bitmap = new Bitmap(inputStream);

			}
		}
		*/
	}
}
