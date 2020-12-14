/*using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItSoftware.Core.HttpHost
{
	using AppFunc = Func<IDictionary<string, object>, Task>;

	public class ItsMiddleware
	{		
		internal AppFunc Middleware(AppFunc nextMiddleware)
		{
			AppFunc appFunc =
			async(IDictionary<string,object> env) =>
			{
				IOwinContext context = new OwinContext( env );

				if ( !context.Environment.ContainsKey( "its.SessionState" ) )
				{
					var state = new Dictionary<string, object>( );
					context.Environment["its.SessionState"] = state;
				}
				
				await this.InvokeDown( context );

				await nextMiddleware( env );

				await this.InvokeUp( context );
			};
			return appFunc;
		}

		protected IDictionary<string,object> SessionState(IOwinContext context)
		{
			if ( context.Environment.ContainsKey( "its.SessionState" ) )
			{
				return context.Environment["its.SessionState"] as IDictionary<string, object>;
			}

			return null;
		}

		protected T GetSessionStateObject<T>(IOwinContext context, string key)
		{
			var session = SessionState( context );
			if ( session.ContainsKey(key) )
			{
				return (T)session[key];
			}
			return default( T );
		}

		protected void SetSessionStateObject<T>( IOwinContext context, string key, T val )
		{
			var session = SessionState( context );
			if ( session.ContainsKey( key ) )
			{
				session.Remove( key );
			}

			session[key] = val;
		}

		protected void SetResponseContentType(IOwinContext context, string type)
		{
			if ( context != null)
			{
				context.Response.Headers["Content-Type"] = type;
			}
		}

		protected void SetResponseContentTypeHtml( IOwinContext context )
		{
			if ( context != null )
			{
				context.Response.Headers["Content-Type"] = "text/html";
			}
		}

		protected void SetResponseContentTypeXml( IOwinContext context )
		{
			if ( context != null )
			{
				context.Response.Headers["Content-Type"] = "text/xml";
			}
		}

		protected void SetResponseContentTypeJson( IOwinContext context )
		{
			if ( context != null )
			{
				context.Response.Headers["Content-Type"] = "application/json";
			}
		}

		protected async Task WriteResponseStringAsync(IOwinContext context, string str)
		{
			if ( context != null )
			{
				await context.Response.WriteAsync( str );
			}
		}

		public virtual async Task InvokeDown( IOwinContext context )
		{
			await Task.Run( ( ) => { } );
		}

		public virtual async Task InvokeUp( IOwinContext context )
		{
			await Task.Run( ( ) => { } );
		}
	}
}
*/