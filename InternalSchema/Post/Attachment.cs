using System.Xml.Serialization;
using Contrib.ExternalImportExport.InternalSchema.Common;

namespace Contrib.ExternalImportExport.InternalSchema.Post {
	/// <summary>
	/// Implementation of the "attachmentType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("attachment")]
	public class Attachment : Node {
		
		/// <summary>
		/// Is this resource embedded?
		/// </summary>
		[XmlAttribute("embedded")]
		public bool Embedded { get; set; }
		
		/// <summary>
		/// The MIME type of the attachment
		/// </summary>
		[XmlAttribute("mime-type")]
		public string MimeType { get; set; }
		
		/// <summary>
		/// The size of the attachment
		/// </summary>
		[XmlAttribute("size")]
		public string Size { get; set; }
		
		/// <summary>
		/// The internal or external URL to the resource
		/// </summary>
		[XmlAttribute("external-uri")]
		public string ExternalURI { get; set; }
		
		/// <summary>
		/// The original URL of the resource
		/// </summary>
		[XmlAttribute("url")]
		public string URL { get; set; }
		
		/// <summary>
		/// The content of the attachment
		/// </summary>
		[XmlText(typeof(string))]
		public string Value { get; set; }
		
		public Attachment() : base() { }
	}
}