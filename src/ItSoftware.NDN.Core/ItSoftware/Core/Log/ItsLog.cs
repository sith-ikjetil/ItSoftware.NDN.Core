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
	public class ItsLogEventArgs : EventArgs
	{
		public ItsLogEntry ItemAdded { get; set; } = null!;
	}

	public delegate void ItsLogItemAddedEventHandler(object sender, ItsLogEventArgs e);

    public class ItsLog
	{
		#region Public Static Properties
		public static ItsLog ApplicationLog { get; set; } = null!;
		#endregion

		#region Public Properties
		public string FileName { get; private set; }
		public ObservableCollection<ItsLogEntry> Entries { get; private set; } = new ObservableCollection<ItsLogEntry>( );
		public string EventLogName { get; private set; }
		public bool AutoSave { get; set; } = true;
		public bool AutoPurge { get; set; } = false;
		public int PurgeLimit { get; set; } = 5000;
		public bool DoLogInformation { get; set; } = true;
		public bool DoLogWarning { get; set; } = true;
		public bool DoLogError { get; set; } = true;
		public bool DoLogDebug { get; set; } = true;
		public bool DoLogOther { get; set; } = true;
		#endregion

		#region Public Events
		public event ItsLogItemAddedEventHandler? ItemAdded;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename"></param>
        public ItsLog( string filename, string eventLogName, bool loadOld )
		{
			this.FileName = filename;
			this.EventLogName = eventLogName;

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

				foreach ( var element in xd.Root?.Elements( "LogEntry" ) ?? Enumerable.Empty<XElement>())
				{
					this.Entries.Add( new ItsLogEntry( element ) );
				}

                if (this.AutoPurge)
                {
                    if (this.Entries.Count >= this.PurgeLimit)
                    {
                        this.Entries.Clear();
                        if (this.AutoSave)
                        {
                            this.SaveLog();
                        }
                    }
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
			this.ToXDocument().Save( filename );
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public XDocument ToXDocument()
        {
            XDocument xd = new XDocument(new XDeclaration("1.0", "utf-8", null));

            XElement xeRoot = new XElement("Log");
            xd.Add(xeRoot);
            xeRoot.SetAttributeValue("Name", this.EventLogName ?? string.Empty);

            foreach (var entry in this.Entries)
            {
                try
                {
                    xeRoot.Add(entry.ToXElement());
                }
                catch (System.Exception)
                {

                }
            }

            return xd;
        }
        /// <summary>
        /// Log information entry.
        /// </summary>
        /// <param name="text"></param>
		public void LogInformation(string title, string text)
        {
			this.LogInformation(title, text, string.Empty);
		}        
		/// <summary>
		/// Log information entry.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="text"></param>
		/// <param name="customText"></param>
        public void LogInformation(string title, string text, string customText)
		{
			if ( !this.DoLogInformation )
            {
				return;
            }

			if (this.AutoPurge)
			{
				if (this.Entries.Count >= this.PurgeLimit )
				{
					this.Entries.Clear();
					if (this.AutoSave)
					{
						this.SaveLog();
					}
				}
			}

			this.Entries.Add( new ItsLogEntry( ) { Text = text, CustomText=customText, Type = ItsLogType.Information, When = DateTime.Now, Title = title } );

			if ( this.AutoSave )
			{
				this.SaveLog( );
			}

            this.ItemAdded?.Invoke(this, new ItsLogEventArgs() { ItemAdded = this.Entries.Last() });
        }
		/// <summary>
		/// Log warning entry.
		/// </summary>
		/// <param name="text"></param>
		public void LogWarning( string title, string text )
		{
			this.LogWarning(title, text, string.Empty);
		}
		/// <summary>
		/// Log warning entry.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="text"></param>
		/// <param name="customText"></param>
        public void LogWarning(string title, string text, string customText)
        {
			if (!this.DoLogWarning)
            {
				return;
            }

            if (this.AutoPurge)
            {
                if (this.Entries.Count >= this.PurgeLimit)
                {
                    this.Entries.Clear();
                    if (this.AutoSave)
                    {
                        this.SaveLog();
                    }
                }
            }

            this.Entries.Add( new ItsLogEntry( ) { Text = text, CustomText=customText, Type = ItsLogType.Warning, When = DateTime.Now, Title = title } );

			if ( this.AutoSave )
			{
				this.SaveLog( );
			}

            this.ItemAdded?.Invoke(this, new ItsLogEventArgs() { ItemAdded = this.Entries.Last() });
        }
		/// <summary>
		/// Log error entry.
		/// </summary>
		/// <param name="text"></param>
		public void LogError( string title, string text )
		{ 
			this.LogError(title, text, string.Empty);
		}
		/// <summary>
		/// Log error entry
		/// </summary>
		/// <param name="title"></param>
		/// <param name="text"></param>
		/// <param name="customText"></param>
		public void LogError(string title, string text, string customText)
		{
			if (!this.DoLogError)
            {
				return;
            }

            if (this.AutoPurge)
            {
                if (this.Entries.Count >= this.PurgeLimit)
                {
                    this.Entries.Clear();
                    if (this.AutoSave)
                    {
                        this.SaveLog();
                    }
                }
            }

            this.Entries.Add( new ItsLogEntry( ) { Text = text, CustomText = customText, Type = ItsLogType.Error, When = DateTime.Now, Title = title } );

			if ( this.AutoSave )
			{
				this.SaveLog( );
			}

            this.ItemAdded?.Invoke(this, new ItsLogEventArgs() { ItemAdded = this.Entries.Last() });
        }
		/// <summary>
		/// Log debug entry.
		/// </summary>
		/// <param name="text"></param>
		public void LogDebug( string title, string text )
		{
			this.LogDebug(title, text, string.Empty);
		}
		/// <summary>
		/// Log debug entry.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="text"></param>
		/// <param name="customText"></param>
		public void LogDebug( string title, string text, string customText)
		{
			if (!this.DoLogDebug)
            {
				return;
            }

            if (this.AutoPurge)
            {
                if (this.Entries.Count >= this.PurgeLimit)
                {
                    this.Entries.Clear();
                    if (this.AutoSave)
                    {
                        this.SaveLog();
                    }
                }
            }

            this.Entries.Add( new ItsLogEntry( ) { Text = text, CustomText = customText, Type = ItsLogType.Debug, When = DateTime.Now, Title = title } );

			if ( this.AutoSave )
			{
				this.SaveLog( );
			}

            this.ItemAdded?.Invoke(this, new ItsLogEventArgs() { ItemAdded = this.Entries.Last() });
        }
		/// <summary>
		/// Log other entry.
		/// </summary>
		/// <param name="text"></param>
		public void LogOther( string title, string text )
		{
			this.LogOther(title, text, string.Empty);
		}
		/// <summary>
		/// Log other entry.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="text"></param>
		/// <param name="customText"></param>
		public void LogOther(string title, string text, string customText)
		{
			if (!this.DoLogOther)
            {
				return;
            }

            if (this.AutoPurge)
            {
                if (this.Entries.Count >= this.PurgeLimit)
                {
                    this.Entries.Clear();
                    if (this.AutoSave)
                    {
                        this.SaveLog();
                    }
                }
            }

            this.Entries.Add( new ItsLogEntry( ) { Text = text, CustomText=customText, Type = ItsLogType.Other, When = DateTime.Now, Title = title } );

			if ( this.AutoSave )
			{
				this.SaveLog( );
			}

            this.ItemAdded?.Invoke(this, new ItsLogEventArgs() { ItemAdded = this.Entries.Last() });
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
