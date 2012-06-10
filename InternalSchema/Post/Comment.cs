using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using Contrib.ImportExport.InternalSchema.Common;

namespace Contrib.ImportExport.InternalSchema.Post {
	/// <summary>
	/// Implmentation of the "commentType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("comment")]
	public class Comment : Node {
		
		/// <summary>
		/// The content of the comment
		/// </summary>
		[XmlElement("content")]
		public Content Content { get; set; }
		
		/// <summary>
		/// The name of the user who left the comment
		/// </summary>
		[XmlAttribute("user-name")]
		public string UserName { get; set; }
		
		/// <summary>
		/// The e-mail address of the user who left the comment
		/// </summary>
		[XmlAttribute("user-email")]
		public string UserEmail { get; set; }
		
		/// <summary>
		/// The URL of the user who left the comment
		/// </summary>
		[XmlAttribute("user-url")]
		public string UserURL { get; set; }
		
		public Comment()
			: base() {
			Content = new Content();
		}
	}
}