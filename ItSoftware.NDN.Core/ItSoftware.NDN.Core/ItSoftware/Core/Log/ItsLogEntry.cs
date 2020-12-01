using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ItSoftware.Core.Log
{
	public class ItsLogEntry
	{
		#region Public Properties
		public ItsLogType Type { get; set; }
		public string Title { get; set; }
		public string Text { get; set; }
		public DateTime When { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public ItsLogEntry( )
		{

		}
		/// <summary>
		/// Deserialize constructor from XElement.
		/// </summary>
		/// <param name="xe"></param>
		public ItsLogEntry( XElement xe )
		{
			this.Type = (ItsLogType) Enum.Parse( typeof( ItsLogType ), xe.Attribute( "Type" ).Value, false );
			this.Title = xe.Attribute( "Title" ).Value;
			this.Text = xe.Value;
			this.When = DateTime.Parse( xe.Attribute( "When" ).Value );
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Serialize to XElement.
		/// </summary>
		/// <returns></returns>
		public XElement ToXElement( )
		{
			XElement xe = new XElement( "LogEntry" );
			xe.SetAttributeValue( "Type", this.Type.ToString( ) );
			xe.SetAttributeValue( "Title", this.Title );
			xe.SetAttributeValue( "When", this.When.ToString( "s" ) );
			xe.Value = this.Text;

			return xe;
		}
		#endregion
	}
}
