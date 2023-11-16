//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using System.Configuration;
//using Microsoft.Data.SqlClient;
//using ItSoftware.Core.Exception;
///// <summary>
///// 
///// </summary>
//namespace ItSoftware.Core.DbClient
//{
//	/// <summary>
//	/// ItsDbClient wraps System.Data.SqlClient db api.
//	/// </summary>
//	public class ItsDbClient : IDisposable
//	{
//		#region Private Member Fields
//		private bool m_isDisposed = false;		
//		private SqlConnection m_sqlConnection = null;
//		private SqlTransaction m_sqlTransaction = null;		
//		#endregion

//		#region Private Static Fields
//		private static int s_commandTimeout = -1;
//		//private static int s_connectionTimeout = -1;		
//		private static string m_activeConnectionString = string.Empty;
//		#endregion

//		#region Public Static Properties		
//		public string ConnectionString { get; private set; } = string.Empty;
		
//		public static string ActiveConnectionString
//		{
//			get
//			{
//				/*if ( !string.IsNullOrEmpty(ItsDbClient.m_activeConnectionString) && !string.IsNullOrWhiteSpace(ItsDbClient.m_activeConnectionString) )
//				{
//					return ItsDbClient.m_activeConnectionString;
//				}

//				string acs = ConfigurationManager.AppSettings["ActiveConnectionString"] ?? string.Empty;
//				if (string.IsNullOrEmpty(acs))
//				{
//					throw new ItsException<ItsDbClientExceptionArgs>(string.Format("No ActiveConnectionString settings in web.config's appSettings section."));
//				}
//				try
//				{
//					ItsDbClient.m_activeConnectionString = ConfigurationManager.ConnectionStrings[acs].ConnectionString;
//					if (string.IsNullOrEmpty(ItsDbClient.m_activeConnectionString) || string.IsNullOrWhiteSpace(ItsDbClient.m_activeConnectionString))
//					{
//						throw new ItsException<ItsDbClientExceptionArgs>(string.Format("No connection string named '{0}' found. Check web.config's connectionStrings section.", acs));
//					}					
//				}
//				catch (NullReferenceException nre)
//				{
//					throw new ItsException<ItsDbClientExceptionArgs>(string.Format("No connection string named '{0}' found. Check web.config's connectionStrings section.", acs), nre);
//				}*/

//				return ItsDbClient.m_activeConnectionString;
//			}
//		}
//		#endregion

//		#region Static Constructors
//		static ItsDbClient()
//		{		
//			/* if (ConfigurationManager.AppSettings["ConnectionTimeout"] != null)
//			 {
//				 DbClient.s_connectionTimeout = int.Parse(ConfigurationManager.AppSettings["ConnectionTimeout"]);
//			 }
//			 */
//			/*if (ConfigurationManager.AppSettings["CommandTimeout"] != null)
//			{
//				ItsDbClient.s_commandTimeout = int.Parse(ConfigurationManager.AppSettings["CommandTimeout"]!);
//			}*/

//		}
//		#endregion

//		#region Public Constructors
//		/// <summary>
//		/// Default constructor
//		/// </summary>
//		public ItsDbClient() : this(false)
//		{

//		}
//		/// <summary>
//		/// Overloaded constructor
//		/// </summary>
//		/// <param name="autoConnect"></param>
//		public ItsDbClient(bool autoConnect)
//		{
//			this.ConnectionString = ItsDbClient.ActiveConnectionString;
//			m_sqlConnection = new SqlConnection(this.ConnectionString);
//			if (autoConnect)
//			{
//				m_sqlConnection.Open();
//			}
//		}
//		/// <summary>
//		/// Overloaded constructor.
//		/// </summary>
//		/// <param name="connectionString"></param>
//		/// <param name="autoConnect"></param>
//		public ItsDbClient(string connectionString, bool autoConnect)
//		{
//			this.ConnectionString = connectionString;
//			m_sqlConnection = new SqlConnection(ConnectionString);
//			if (autoConnect)
//			{
//				m_sqlConnection.Open();
//			}
//		}
//		/// <summary>
//		/// Overloaded constructor.
//		/// </summary>
//		/// <param name="connectionString"></param>
//		public ItsDbClient(string connectionString)
//			: this(connectionString, false)
//		{
//		}
//		/// <summary>
//		/// Finalizer
//		/// </summary>
//		~ItsDbClient()
//		{
//			this.Disposing(false);
//		}
//		#endregion

//		#region Public Properties
//		/// <summary>
//		/// Connection State
//		/// </summary>
//		public ConnectionState State
//		{
//			get
//			{
//				return m_sqlConnection.State;
//			}
//		}
//		#endregion

//		#region Public Methods
//		/// <summary>
//		/// Open
//		/// </summary>
//		public void Open()
//		{
//			if ( this.m_isDisposed )
//			{
//				throw new ObjectDisposedException("DbClient");
//			}
//			m_sqlConnection.Open();
//		}
//		/// <summary>
//		/// Close
//		/// </summary>
//		public void Close()
//		{
//			if (!this.m_isDisposed)
//			{
//				m_sqlConnection.Close();
//			}
//		}
//		/// <summary>
//		/// BeginTransaction
//		/// </summary>
//		public void BeginTransaction()
//		{
//			if (this.m_isDisposed)
//			{
//				throw new ObjectDisposedException("DbClient");
//			}

//			if (m_sqlTransaction != null)
//			{
//				throw new ItsException<ItsDbClientExceptionArgs>("Transaction already active");
//			}
//			m_sqlTransaction = m_sqlConnection.BeginTransaction();
//		}
//		/// <summary>
//		/// Commit
//		/// </summary>
//		public void Commit()
//		{
//			if (this.m_isDisposed)
//			{
//				throw new ObjectDisposedException("DbClient");
//			}

//			if (m_sqlTransaction == null)
//			{
//				throw new ItsException<ItsDbClientExceptionArgs>("Not in a transaction");
//			}
//			m_sqlTransaction.Commit();
//			m_sqlTransaction = null;
//		}
//		/// <summary>
//		/// Rollback
//		/// </summary>
//		public void Rollback()
//		{
//			if (this.m_isDisposed)
//			{
//				throw new ObjectDisposedException("DbClient");
//			}

//			if (m_sqlTransaction == null)
//			{
//				throw new ItsException<ItsDbClientExceptionArgs>("Not in a transaction");
//			}
//			m_sqlTransaction.Rollback();
//			m_sqlTransaction = null;
//		}
//		/// <summary>
//		/// ExecuteNonQuery
//		/// </summary>
//		/// <param name="sql"></param>
//		/// <returns></returns>
//		public int ExecuteNonQuery(string sql)
//		{
//			if (this.m_isDisposed)
//			{
//				throw new ObjectDisposedException("DbClient");
//			}

//			return CreateCommand(sql).ExecuteNonQuery();
//		}
//		/// <summary>
//		/// ExecuteQuery
//		/// </summary>
//		/// <param name="sql"></param>
//		/// <returns></returns>
//		public SqlDataReader ExecuteQuery(string sql)
//		{
//			if (this.m_isDisposed)
//			{
//				throw new ObjectDisposedException("DbClient");
//			}

//			return CreateCommand(sql).ExecuteReader();
//		}
//		/// <summary>
//		/// ExecuteScalar
//		/// </summary>
//		/// <param name="sql"></param>
//		/// <returns></returns>
//		public object ExecuteScalar(string sql)
//		{
//			if (this.m_isDisposed)
//			{
//				throw new ObjectDisposedException("DbClient");
//			}

//			return CreateCommand(sql).ExecuteScalar();
//		}
//		#endregion

//		#region Private Methods
//		/// <summary>
//		/// CreateCommand
//		/// </summary>
//		/// <param name="sql"></param>
//		/// <returns></returns>
//		public SqlCommand CreateCommand(string sql)
//		{
//			if (this.m_isDisposed)
//			{
//				throw new ObjectDisposedException("DbClient");
//			}

//			SqlCommand sqlCommand = m_sqlConnection.CreateCommand();
//			sqlCommand.Transaction = m_sqlTransaction;
//			if (ItsDbClient.s_commandTimeout > 0)
//			{
//				sqlCommand.CommandTimeout = ItsDbClient.s_commandTimeout;
//			}
//			sqlCommand.CommandText = sql;
//			sqlCommand.CommandType = CommandType.Text;
//			return sqlCommand;
//		}
//		#endregion

//		#region IDisposable Members
//		/// <summary>
//		/// Dispose
//		/// </summary>
//		public void Dispose()
//		{
//			if (!this.m_isDisposed)
//			{
//				this.Disposing(true);
//			}
//		}

//		#endregion

//		/// <summary>
//		/// Disposing
//		/// </summary>
//		/// <param name="disposing"></param>
//		private void Disposing(bool disposing)
//		{
//			this.m_isDisposed = true;

//			if (disposing)
//			{
//				GC.SuppressFinalize(this);
//			}

//			if (m_sqlTransaction != null)
//			{
//				m_sqlTransaction.Rollback();
//			}
//			if (m_sqlConnection != null)
//			{
//				this.Close();
//				this.m_sqlConnection = null;
//			}
//		}
//	}// class
//}// namespace
