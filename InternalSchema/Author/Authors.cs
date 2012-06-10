using System.Collections.Generic;
using System.Xml.Serialization;

namespace Contrib.ImportExport.InternalSchema.Author {
	/// <summary>
	/// Implementation of the "authorsType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("authors")]
	public class Authors {
		
		/// <summary>
		/// A list of authors
		/// </summary>
		[XmlElement("author")]
		public List<Author> AuthorList { get; set; }
		
		public Authors() {
			AuthorList = new List<Author>();
		}
	}
}