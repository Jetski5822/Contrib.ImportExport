using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Contrib.ExternalImportExport.InternalSchema.Common;

namespace Contrib.ExternalImportExport.InternalSchema.Category {
	/// <summary>
	/// Implementation of "categoryType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("category")]
	public class Category : Node {
		
		/// <summary>
		/// A reference to the parent category
		/// </summary>
		[XmlAttribute("parentref")]
		public string ParentCategory { get; set; }
		
		/// <summary>
		/// The description of the category
		/// </summary>
		[XmlAttribute("description")]
		public string Description { get; set; }
		
		public Category() : base() { }
	}
}