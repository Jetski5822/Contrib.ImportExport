using System.Linq;
using System.Xml.Linq;

namespace Contrib.ImportExport.Providers.Wordpress {
    internal class WordpressNamespaces {
        private XNamespace _wpNamespace;
        protected internal XNamespace ExcerptNamespace { get; set; }
        protected internal XNamespace ContentNamespace { get; set; }
        protected internal XNamespace WfwNamespace { get; set; }
        protected internal XNamespace DcNamespace { get; set; }
        protected internal XNamespace WpNamespace {
            get { return _wpNamespace; }
            set {
                var internalValue = value.ToString().Replace("http://wordpress.org/export/", string.Empty);

                if (internalValue.EndsWith("/"))
                    internalValue = internalValue.Substring(0, internalValue.Length - 1);

                _wpNamespace = internalValue;
            }
        }
    }
}