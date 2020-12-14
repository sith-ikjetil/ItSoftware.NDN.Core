/*using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItSoftware.Core.HttpHost
{
	using AppFunc = Func<IDictionary<string, object>, Task>;

	public enum ItsHttpHostStatus
	{
		Inactive,
		Running,
		Disposed
	}

	public class ItsHttpHost : IDisposable
	{		
		private IDisposable m_pIDisposableWebApp;
		public int Port { get; private set; } = 5000;
		public ItsHttpHostStatus Status { get; private set; } = ItsHttpHostStatus.Inactive;

		public ItsHttpHost(int port)
		{
			this.Port = port;
		}	

		public void Start(List<ItsMiddleware> list)
		{
			if ( list == null )
			{
				throw new ArgumentNullException( "list" );
			}
			
			if ( list.Count == 0 )
			{
				throw new ArgumentException( "list contains no elements" );
			}

			if ( this.Status == ItsHttpHostStatus.Disposed || disposedValue )
			{
				throw new ObjectDisposedException( "ItsHttpHost" );
			}			

			this.m_pIDisposableWebApp = WebApp.Start( $"http://localhost:{this.Port}", (app) => 
			{				
				foreach ( var m in list )
				{
					var middleware = new Func<AppFunc, AppFunc>( m.Middleware );
					app.Use( middleware );
				}
			} );

			this.Status = ItsHttpHostStatus.Running;
		}

		public void Stop()
		{
			this.Dispose( );
		}

		#region IDisposable Support
		private bool disposedValue = false;

		protected virtual void Dispose( bool disposing )
		{
			if ( !disposedValue )
			{
				if ( disposing )
				{
					if ( this.m_pIDisposableWebApp != null )
					{
						this.m_pIDisposableWebApp.Dispose( );
						this.Status = ItsHttpHostStatus.Disposed;						
					}
				}
				disposedValue = true;
			}			
		}
		
		public void Dispose( )
		{
			Dispose( true );
		}
		#endregion
	}
}
*/