using System.Collections.Generic;
using System.Xml.Serialization;

namespace Contrib.ImportExport.InternalSchema.Post.Additional {
	/// <summary>
	/// Implementation of the "categoriesRefType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("tags")]
	public class TagReferences {
		
		/// <summary>
		/// The categories for a post
		/// </summary>
		[XmlElement("tags")]
		public List<TagReference> TagReferenceList { get; set; }

        public TagReferences() {
			TagReferenceList = new List<TagReference>();
		}
	}
}