using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using Contrib.ImportExport.InternalSchema.Common;

namespace Contrib.ImportExport.InternalSchema.Post {
	/// <summary>
	/// Implementation of the "trackbackType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("trackback")]
	public class Trackback : Node {
		
		/// <summary>
		/// The URL of the trackback
		/// </summary>
		[XmlAttribute("url")]
		public string Url { get; set; }
		
		public Trackback() : base() { }
	}
}