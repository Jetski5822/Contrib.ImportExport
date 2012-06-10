using System.Collections.Generic;
using System.Xml.Serialization;

namespace Contrib.ExternalImportExport.InternalSchema.Category {
	/// <summary>
	/// Implementation of the "categoriesType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("categories")]
	public class Categories {
		
		/// <summary>
		/// The list of categories for the blog
		/// </summary>
		[XmlElement("category")]
		public List<Category> CategoryList { get; set; }
		
		public Categories() {
			CategoryList = new List<Category>();
		}
	}
}