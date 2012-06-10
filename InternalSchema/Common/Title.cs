using System.Xml.Serialization;

namespace Contrib.ExternalImportExport.InternalSchema.Common {
	/// <summary>
	/// Implementation of the "titleType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("title")]
	public class Title {
		
		/// <summary>
		/// The type of content in the title
		/// </summary>
		[XmlAttribute("type")]
		public string Type { get; set; }
		
		/// <summary>
		/// The value of the title element
		/// </summary>
		[XmlText(typeof(string))]
		public string Value { get; set; }
		
		public Title() { }
	}
}