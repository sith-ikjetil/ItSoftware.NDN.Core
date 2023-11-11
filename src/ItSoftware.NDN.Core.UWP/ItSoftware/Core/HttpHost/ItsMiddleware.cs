/*using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Owin;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItSoftware.Core.HttpHost
{

	public class ItsMiddleware
	{
		protected IDictionary<string,object> SessionState(OwinFeatureCollection context)
		{
			if ( context.Environment.ContainsKey( "its.SessionState" ) )
			{
				return context.Environment["its.SessionState"] as IDictionary<string, object>;
			}

			return null;
		}

		protected T GetSessionStateObject<T>(OwinFeatureCollection context, string key)
		{
			var session = SessionState( context );
			if ( session.ContainsKey(key) )
			{
				return (T)session[key];
			}
			return default( T );
		}

		protected void SetSessionStateObject<T>(OwinFeatureCollection context, string key, T val )
		{
			var session = SessionState( context );
			if ( session.ContainsKey( key ) )
			{
				session.Remove( key );
			}

			session[key] = val;
		}

		protected void SetResponseContentType(HttpContext context, string type)
		{
			if ( context != null)
			{
				context.Response.Headers["Content-Type"] = type;
			}
		}

		protected void SetResponseContentTypeHtml(HttpContext context )
		{
			if ( context != null )
			{
				context.Response.Headers["Content-Type"] = "text/html";
			}
		}

		protected void SetResponseContentTypeXml(HttpContext context )
		{
			if ( context != null )
			{
				context.Response.Headers["Content-Type"] = "text/xml";
			}
		}

		protected void SetResponseContentTypeJson(HttpContext context )
		{
			if ( context != null )
			{
				context.Response.Headers["Content-Type"] = "application/json";
			}
		}

		protected async Task WriteResponseStringAsync(HttpContext context, string str)
		{
			if ( context != null )
			{
				await context.Response.WriteAsync( str );
			}
		}

		public virtual async Task InvokeDown(HttpContext context )
		{
			await Task.Run( ( ) => { } );
		}

		public virtual async Task InvokeUp(HttpContext context )
		{
			await Task.Run( ( ) => { } );
		}
	}
}
*/