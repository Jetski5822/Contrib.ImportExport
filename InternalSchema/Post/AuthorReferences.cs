using System.Collections.Generic;
using System.Xml.Serialization;

namespace Contrib.ImportExport.InternalSchema.Post {
	/// <summary>
	/// Implementation of the "authorsRefType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("authors")]
	public class AuthorReferences {
		
		/// <summary>
		/// The authors for the post
		/// </summary>
		[XmlElement("author")]
		public List<AuthorReference> AuthorReferenceList { get; set; }
		
		public AuthorReferences() {
			AuthorReferenceList = new List<AuthorReference>();
		}
	}
}