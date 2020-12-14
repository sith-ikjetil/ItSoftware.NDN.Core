using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
namespace ItSoftware.Core.Azure
{
	public class AzureFunctionResult
	{
		public string Content { get; private set; }
		public AzureFunctionResult(string payload)
		{
			this.Content = payload;
		}

		public HttpResponseMessage AsHtml()
		{
			var response = new HttpResponseMessage(HttpStatusCode.OK);
			response.Content = new StringContent(this.Content);
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return response;
		}

		public HttpResponseMessage AsXml()
		{
			var response = new HttpResponseMessage(HttpStatusCode.OK);
			response.Content = new StringContent(this.Content);
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
			return response;
		}

		public HttpResponseMessage AsJson()
		{
			var response = new HttpResponseMessage(HttpStatusCode.OK);
			response.Content = new StringContent(this.Content);
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			return response;
		}

		public HttpResponseMessage AsText()
		{
			var response = new HttpResponseMessage(HttpStatusCode.OK);
			response.Content = new StringContent(this.Content);
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
			return response;
		}
	}
}
