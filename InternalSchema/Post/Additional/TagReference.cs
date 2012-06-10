using System.Xml.Linq;
using System.Xml.Serialization;

namespace Contrib.ExternalImportExport.InternalSchema.Post.Additional {
	/// <summary>
	/// Implementation of the "categoryRefType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("tag")]
	public class TagReference {
		
		/// <summary>
		/// The ID of the category
		/// </summary>
		[XmlAttribute("ref")]
		public string ID { get; set; }

        public string Title { get; set; }

		public TagReference() { }
		
		/// <summary>
		/// Constructor for a filled category reference.
		/// </summary>
		/// <param name="reference">
		/// The WXR category element
		/// </param>
        public TagReference(XElement reference) {
			ID = reference.Attribute("nicename").Value.Trim();
            Title = reference.Value.Trim();
		}
	}
}