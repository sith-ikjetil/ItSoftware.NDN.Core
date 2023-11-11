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
	public class ItsMiddlewareHost
	{
		internal List<ItsMiddleware> List { get; set; }
		public ItsMiddlewareHost(List<ItsMiddleware> list)
		{
			if ( list == null )
			{
				throw new ArgumentNullException(nameof(list));
			}

			if ( list.Count < 1 )
			{
				throw new ArgumentOutOfRangeException(nameof(list));
			}

			this.List = list;
		}

		internal RequestDelegate Middleware(RequestDelegate nextMiddleware)
		{
			RequestDelegate appFunc =
			async(HttpContext context) =>
			{
				var environment = new OwinEnvironment(context);
				var features = new OwinFeatureCollection(environment);

				if ( !features.Environment.ContainsKey( "its.SessionState" ) )
				{
					var state = new Dictionary<string, object>( );
					features.Environment["its.SessionState"] = state;
				}

				await this.InvokeAt(context, 0);
			};
			return appFunc;
		}

		internal async Task InvokeAt(HttpContext context, int i)
		{
			if (i >= this.List.Count )
			{
				return;
			}

			await this.List[i].InvokeDown(context);

			await this.InvokeAt(context, i + 1);

			await this.List[i].InvokeUp(context);
		}

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