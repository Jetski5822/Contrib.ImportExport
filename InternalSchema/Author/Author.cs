using System.Xml.Serialization;
using Contrib.ImportExport.InternalSchema.Common;

namespace Contrib.ImportExport.InternalSchema.Author {
	/// <summary>
	/// An author of posts within a blog.
	/// </summary>
	[XmlRoot("author")]
	public class Author : Node {
		
		/// <summary>
		/// The e-mail address for the author
		/// </summary>
		[XmlAttribute("email")]
		public string Email { get; set; }
		
		public Author() : base() {
		}
	}
}