using System.Collections.Generic;
using System.Xml.Serialization;

namespace Contrib.ExternalImportExport.InternalSchema.Post {
	/// <summary>
	/// Implementation of the "commentsType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("comments")]
	public class Comments {
		
		/// <summary>
		/// The comments on the post
		/// </summary>
		[XmlElement("comment")]
		public List<Comment> CommentList { get; set; }

        /// <summary>
        /// Are comments enabled on blog post
        /// </summary>
        [XmlElement("comment_status")]
        public bool Enabled { get; set; }

		public Comments() {
			CommentList = new List<Comment>();
		}
	}
}