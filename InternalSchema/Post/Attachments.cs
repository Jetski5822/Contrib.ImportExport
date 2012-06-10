using System.Collections.Generic;
using System.Xml.Serialization;

namespace Contrib.ImportExport.InternalSchema.Post {
	/// <summary>
	/// Implementation of the "attachmentsType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("attachments")]
	public class Attachments {
		
		/// <summary>
		/// The list of attachments for the post
		/// </summary>
		[XmlElement("attachment")]
		public List<Attachment> AttachmentList { get; set; }
		
		public Attachments() {
			AttachmentList = new List<Attachment>();
		}
	}
}