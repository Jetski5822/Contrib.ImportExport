using System.Xml.Serialization;

namespace Contrib.ImportExport.InternalSchema.Post {
	/// <summary>
	/// Implementation of the "authorRefType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("author")]
	public class AuthorReference {
		
		/// <summary>
		/// The ID of the author
		/// </summary>
		[XmlAttribute("ref")]
		public string ID { get; set; }
		
		public AuthorReference() { }
	}
}