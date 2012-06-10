using System.Xml.Linq;

namespace Contrib.ImportExport.Providers.Wordpress {
    internal class WordpressNamespaces {
        protected internal XNamespace ExcerptNamespace { get; set; }
        protected internal XNamespace ContentNamespace { get; set; }
        protected internal XNamespace WfwNamespace { get; set; }
        protected internal XNamespace DcNamespace { get; set; }
        protected internal XNamespace WpNamespace { get; set; }
    }
}