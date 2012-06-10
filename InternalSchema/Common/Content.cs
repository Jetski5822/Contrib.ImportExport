using System;
using System.Xml.Serialization;

namespace Contrib.ImportExport.InternalSchema.Common {
	/// <summary>
	/// Implementation of "contentType" from the BlogML XSD.
	/// </summary>
	[XmlRoot("content")]
	public class Content {
		
		public static string TypeHTML = "html";
		public static string TypeXHTML = "xhtml";
		public static string TypeText = "text";
		public static string TypeBase64 = "base64";
		
		/// <summary>
		/// The type of content (use the "Type*" strings defined in this class).
		/// </summary>
		[XmlAttribute("type")]
		public string Type {
			get {
				return type;
			}
			set {
				if ((!value.Equals(TypeHTML))
				    && (!value.Equals(TypeXHTML))
				    && (!value.Equals(TypeText))
				    && !(value.Equals(TypeBase64))) {
					throw new NotSupportedException("Content type " + value + " is not supported in BlogML");
				}
				type = value;
			}
		}
		
		// Actual type variable.
		private string type;
		
		/// <summary>
		/// The content.
		/// </summary>
		[XmlText(typeof(string))]
		public string Value { get; set; }
		
		public Content() { }
	}
}