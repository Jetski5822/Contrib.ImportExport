using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using Contrib.ExternalImportExport.InternalSchema.Common;

namespace Contrib.ExternalImportExport.InternalSchema.Post {
	/// <summary>
	/// Implementation of the "trackbackType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("trackback")]
	public class Trackback : Node {
		
		/// <summary>
		/// The URL of the trackback
		/// </summary>
		[XmlAttribute("url")]
		public string URL { get; set; }
		
		public Trackback() : base() { }
	}
}