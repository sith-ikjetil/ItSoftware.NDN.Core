/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItSoftware.NDN.Core.TestApplication
{
	using ItSoftware.Core.HttpHost;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Owin;

	public class Middleware1 : ItsMiddleware
	{		
		public override async Task InvokeDown( HttpContext context )
		{ 			
			base.SetResponseContentTypeHtml( context );
			await base.WriteResponseStringAsync( context, "<h1>Middleware 1 - Down</h1>" );
		}

		public override async Task InvokeUp(HttpContext context )
		{
			await base.WriteResponseStringAsync( context, "<h1>Middleware 1 - Up</h1>" );
		}
	}
}
*/