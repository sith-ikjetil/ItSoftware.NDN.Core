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
		public ItsLogType Type { get; set; } = ItsLogType.Information;
		public string Title { get; set; } = null;
		public string Text { get; set; } = null;
		public string Details { get; set; } = null;
		public DateTime When { get; set; } = DateTime.MinValue;
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
			this.Type = (ItsLogType) Enum.Parse( typeof( ItsLogType ), xe.Attribute( "Type" )?.Value ?? ItsLogType.Information.ToString(), false );
			this.Title = xe.Attribute( "Title" )?.Value ?? String.Empty;			
			this.When = DateTime.Parse( xe.Attribute( "When" )?.Value ?? DateTime.Now.ToString("s"));

			if (xe.Element("Text") != null)
			{
                this.Text = xe.Element("Text")?.Value ?? string.Empty;
            }
			else
			{
				this.Text = string.Empty;
			}
			
            if (xe.Element("Details") != null )
			{
				this.Details = xe.Element("Details")?.Value ?? string.Empty;
			}
            else
            {
				this.Details = string.Empty;
            }
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

            XElement xeText = new XElement("Text");
            xeText.Value = this.Text ?? string.Empty;
            xe.Add(xeText);
			
			XElement xeDetails = new XElement("Details");
            xeDetails.Value = this.Details ?? string.Empty;
			xe.Add(xeDetails);

			return xe;
		}
		#endregion
	}
}
