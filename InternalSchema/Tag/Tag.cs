using System.Xml.Serialization;
using Contrib.ExternalImportExport.InternalSchema.Common;

namespace Contrib.ExternalImportExport.InternalSchema.Tag {
	/// <summary>
	/// Implementation of "categoryType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("tag")]
	public class Tag : Node {

        /// <summary>
        /// The description of the slug
        /// </summary>
        [XmlAttribute("slug")]
        public string Slug { get; set; }

		public Tag() : base() { }
	}
}