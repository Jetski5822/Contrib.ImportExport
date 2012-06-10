using System;
using System.Xml.Serialization;

namespace Contrib.ImportExport.InternalSchema.Common {
	/// <summary>
	/// Implementation of the "nodeType" from the BlogML XSD.
	/// </summary>
	public abstract class Node {
		
		/// <summary>
		/// The unique identifier for this node.
		/// </summary>
		[XmlAttribute("id")]
		public string ID { get; set; }
		
		/// <summary>
		/// The title for this node.
		/// </summary>
		[XmlElement("title")]
		public string Title { get; set; }
		
		/// <summary>
		/// The date/time this node was created (optional).
		/// </summary>
		[XmlAttribute("date-created")]
        public DateTime DateCreated { get; set; }
		
		/// <summary>
		/// The date/time this node was last updated (optional). 
		/// </summary>
        [XmlAttribute("date-modified")]
        public DateTime DateModified { get; set; }

	    /// <summary>
		/// Whether this node is published/approved (optional).
		/// </summary>
		[XmlAttribute("approved")]
		public bool Approved { get; set; }
		
		/// <summary>
		/// Create the node (currently does nothing)
		/// </summary>
		public Node() { }
	}
}