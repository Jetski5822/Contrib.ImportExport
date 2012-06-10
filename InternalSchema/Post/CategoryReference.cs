using System.Xml.Linq;
using System.Xml.Serialization;

namespace Contrib.ExternalImportExport.InternalSchema.Post {
	/// <summary>
	/// Implementation of the "categoryRefType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("category")]
	public class CategoryReference {
		
		/// <summary>
		/// The ID of the category
		/// </summary>
		[XmlAttribute("ref")]
		public string ID { get; set; }
		
		public CategoryReference() { }
	}
}