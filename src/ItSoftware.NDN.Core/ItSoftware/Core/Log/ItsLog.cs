using ItSoftware.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ItSoftware.Core.Log
{
	public class ItsLog
	{
		#region Public Static Properties
		public static ItsLog ApplicationLog { get; set; }
		#endregion

		#region Public Properties
		public string FileName { get; private set; }
		public ObservableCollection<ItsLogEntry> Entries { get; private set; } = new ObservableCollection<ItsLogEntry>( );
		public string EventLogSourceName { get; private set; }
		//public bool ReportToEventLog { get; set; } = true;
		public bool AutoSave { get; set; } = true;
		public bool DoLogInformation { get; set; } = true;
		public bool DoLogWarning { get; set; } = true;
		public bool DoLogError { get; set; } = true;
		public bool DoLogDebug { get; set; } = true;
		public bool DoLogOther { get; set; } = true;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="filename"></param>
		public ItsLog( string filename, string eventLogSourceName, bool loadOld )
		{
			this.FileName = filename;
			this.EventLogSourceName = eventLogSourceName;

			if ( loadOld )
			{
				if ( File.Exists( this.FileName ) )
				{
					LoadLog( this.FileName );
				}
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Clears log.
		/// </summary>
		public void ClearLog( )
		{
			this.Entries.Clear( );
		}
		/// <summary>
		/// Loads log from file.
		/// </summary>
		/// <param name="filename"></param>
		public void LoadLog( string filename )
		{
			this.ClearLog( );

			try
			{
				XDocument xd = XDocument.Load( filename );

				foreach ( var element in xd.Root.Elements( "LogEntry" ) )
				{
					this.Entries.Add( new ItsLogEntry( element ) );
				}
			}
			catch ( System.Exception x )
			{
				this.ClearLog( );
				this.LogError( "ItsLog.LoadLog", x.ItsRenderException( ) );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public void SaveLog( )
		{
			this.SaveLog( this.FileName );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		public void SaveLog( string filename )
		{
			XDocument xd = new XDocument( new XDeclaration( "1.0", "utf-8", null ) );

			XElement xeRoot = new XElement( "Log" );
			xd.Add( xeRoot );
			xeRoot.SetAttributeValue("Name", this.EventLogSourceName ?? string.Empty);

			foreach ( var entry in this.Entries )
			{
				try
				{
					xeRoot.Add( entry.ToXElement( ) );
				}
				catch ( System.Exception )
				{

				}
			}

			xd.Save( filename );
		}
		/// <summary>
		/// Log information entry.
		/// </summary>
		/// <param name="text"></param>
		public void LogInformation( string title, string text )
		{
			if ( !this.DoLogInformation )
            {
				return;
            }

			//if ( this.ReportToEventLog )
			//{
			//	EventLog.WriteEntry( this.EventLogSourceName, $"{title}{Environment.NewLine}{text}", EventLogEntryType.Information );
			//}			

			this.Entries.Add( new ItsLogEntry( ) { Text = text, Type = ItsLogType.Information, When = DateTime.Now, Title = title } );

			if ( this.AutoSave )
			{
				this.SaveLog( );
			}
		}
		/// <summary>
		/// Log warning entry.
		/// </summary>
		/// <param name="text"></param>
		public void LogWarning( string title, string text )
		{
			if (!this.DoLogWarning)
            {
				return;
            }

			//if ( this.ReportToEventLog )
			//{ 
			//	EventLog.WriteEntry( this.EventLogSourceName, $"{title}{Environment.NewLine}{text}", EventLogEntryType.Warning );
			//}			

			this.Entries.Add( new ItsLogEntry( ) { Text = text, Type = ItsLogType.Warning, When = DateTime.Now, Title = title } );

			if ( this.AutoSave )
			{
				this.SaveLog( );
			}
		}
		/// <summary>
		/// Log error entry.
		/// </summary>
		/// <param name="text"></param>
		public void LogError( string title, string text )
		{
			if (!this.DoLogError)
            {
				return;
            }

			//if ( this.ReportToEventLog )
			//{
			//	EventLog.WriteEntry( this.EventLogSourceName, $"{title}{Environment.NewLine}{text}", EventLogEntryType.Error );
			//}			

			this.Entries.Add( new ItsLogEntry( ) { Text = text, Type = ItsLogType.Error, When = DateTime.Now, Title = title } );

			if ( this.AutoSave )
			{
				this.SaveLog( );
			}
		}
		/// <summary>
		/// Log debug entry.
		/// </summary>
		/// <param name="text"></param>
		public void LogDebug( string title, string text )
		{
			if (!this.DoLogDebug)
            {
				return;
            }

			this.Entries.Add( new ItsLogEntry( ) { Text = text, Type = ItsLogType.Debug, When = DateTime.Now, Title = title } );

			if ( this.AutoSave )
			{
				this.SaveLog( );
			}
		}
		/// <summary>
		/// Log other entry.
		/// </summary>
		/// <param name="text"></param>
		public void LogOther( string title, string text )
		{
			if (!this.DoLogOther)
            {
				return;
            }

			this.Entries.Add( new ItsLogEntry( ) { Text = text, Type = ItsLogType.Other, When = DateTime.Now, Title = title } );

			if ( this.AutoSave )
			{
				this.SaveLog( );
			}
		}

		public override string ToString()
		{
			StringBuilder txt = new StringBuilder();
			foreach ( var i in this.Entries )
			{
				txt.AppendLine($"{Enum.GetName(typeof(ItsLogType),i.Type)} : {i.When.ToString("s")} : {i.Title.Replace(":",",")} : {i.Text.Replace(":",",")}");
			}
			return txt.ToString();
		}
		#endregion
	}
}
