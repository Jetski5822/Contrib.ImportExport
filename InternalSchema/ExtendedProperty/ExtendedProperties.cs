using System.Collections.Generic;
using System.Xml.Serialization;

namespace Contrib.ImportExport.InternalSchema.ExtendedProperty {
	/// <summary>
	/// The extended properties for a blog
	/// </summary>
	[XmlRoot("extended-properties")]
	public class ExtendedProperties {
		
		/// <summary>
		/// The extended properties for the blog
		/// </summary>
		[XmlElement("property")]
		public List<ExtendedProperty> ExtendedPropertyList { get; set; }
		
		public ExtendedProperties() {
			ExtendedPropertyList = new List<ExtendedProperty>();
		}
	}
}