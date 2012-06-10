using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Contrib.ExternalImportExport.InternalSchema.Common;

namespace Contrib.ExternalImportExport.InternalSchema.Post {
	/// <summary>
	/// Implementation of the "postType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("post")]
	public class Post : Node {
		
		/// <summary>
		/// The content of the blog post
		/// </summary>
		[XmlElement("content")]
		public Content Content { get; set; }
		
		/// <summary>
		/// The name of the post
		/// </summary>
		[XmlElement("post-name")]
		public Title PostName { get; set; }
		
		/// <summary>
		/// The excerpt for the blog post
		/// </summary>
		[XmlElement("excerpt")]
		public Content Excerpt { get; set; }
		
		/// <summary>
		/// Categories for this post
		/// </summary>
		[XmlElement("categories")]
		public CategoryReferences Categories { get; set; }
		
		/// <summary>
		/// Comments on this post
		/// </summary>
		[XmlElement("comments")]
		public Comments Comments { get; set; }
		
		/// <summary>
		/// Trackbacks for this blog post
		/// </summary>
		[XmlElement("trackbacks")]
		public Trackbacks Trackbacks { get; set; }
		
		/// <summary>
		/// Attachments for this blog post
		/// </summary>
		[XmlElement("attachments")]
		public Attachments Attachments { get; set; }
		
		/// <summary>
		/// Authors for this post
		/// </summary>
		[XmlElement("authors")]
		public AuthorReferences Authors { get; set; }
		
		/// <summary>
		/// The URL of this post
		/// </summary>
		[XmlAttribute("post-url")]
		public string PostURL { get; set; }
		
		/// <summary>
		/// The type of this post
		/// </summary>
		[XmlAttribute("type")]
		public string Type { get; set; }
		
		/// <summary>
		/// Whether this post has an excerpt
		/// </summary>
		[XmlAttribute("hasexcerpt")]
		public bool HasExcerpt { get; set; }
		
		/// <summary>
		/// The number of views of this post
		/// </summary>
		[XmlAttribute("views")]
		public string Views { get; set; }
		
		public Post() : base() {
			// These are the only required item.
			Content = new Content();
			Authors = new AuthorReferences();
		}
	}
}