using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using Contrib.ExternalImportExport.InternalSchema.Author;
using Contrib.ExternalImportExport.InternalSchema.Category;
using Contrib.ExternalImportExport.InternalSchema.Common;
using Contrib.ExternalImportExport.InternalSchema.ExtendedProperty;
using Contrib.ExternalImportExport.InternalSchema.Post;

namespace Contrib.ExternalImportExport.InternalSchema {
	/// <summary>
	/// C# representation of a complete BlogML document.
	/// </summary>
	[XmlRoot(Namespace = "http://www.blogml.com/2006/09/BlogML", ElementName = "blog")]
	public class Blog {
		
		/// <summary>
		/// The date/time the export was created
		/// </summary>
		[XmlAttribute("date-created")]
		public string DateCreated { get; set; }
		
		/// <summary>
		/// The root URL of the blog being exported
		/// </summary>
		[XmlAttribute("root-url")]
		public string RootURL { get; set; }
		
		/// <summary>
		/// The main title of the blog
		/// </summary>
		[XmlElement("title")]
		public Title Title { get; set; }
		
		/// <summary>
		/// The subtitle/tag line of the blog
		/// </summary>
		[XmlElement("sub-title")]
		public Title SubTitle { get; set; }
		
		/// <summary>
		/// A list of authors in the blog
		/// </summary>
		[XmlElement("authors")]
		public Authors Authors { get; set; }
		
		/// <summary>
		/// Extended properties for the blog
		/// </summary>
		[XmlElement("extended-properties")]
		public ExtendedProperties ExtendedProperties { get; set; }
		
		/// <summary>
		/// A list of categories for the blog
		/// </summary>
		[XmlElement("categories")]
		public Categories Categories { get; set; }
		
		/// <summary>
		/// A list of posts for the blog
		/// </summary>
		[XmlElement("posts")]
		public Posts Posts { get; set; }
		
		/// <summary>
		/// The number of posts in this blog
		/// </summary>
		public int PostCount {
			get {
				return (null == Posts) ? 0 : this.Posts.PostList.Count;
			}
		}
		
		/// <summary>
		/// The number of comments on posts in this blog
		/// </summary>
		public int CommentCount {
			get {
				IEnumerable<Post.Post> posts = from p in Posts.PostList where p.Comments != null select p;
				
				int count = 0;
				foreach (Post.Post post in posts) count += post.Comments.CommentList.Count;
				return count;
			}
		}
		
		/// <summary>
		/// The number of trackbacks on posts in this blog
		/// </summary>
		public int TrackbackCount {
			get {
				IEnumerable<Post.Post> posts = from p in Posts.PostList where p.Trackbacks != null select p;
				
				int count = 0;
				foreach(Post.Post post in posts) count += post.Trackbacks.TrackbackList.Count;
				return count;
			}
		}
				
		
		/// <summary>
		/// Create a new, empty blog representation
		/// </summary>
		public Blog() {
			DateCreated = DateTime.Now.ToString("s");
			Title = new Title();
			SubTitle = new Title();
			Authors = new Authors();
			Categories = new Categories();
			Posts = new Posts();
		}
	}
}