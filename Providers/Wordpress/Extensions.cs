using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Contrib.ImportExport.Providers.Wordpress {
    internal static class Extensions {
        internal static XElement WordpressElement(this XElement element, WordpressNamespaces namespaces, string name) {
            return element.Element(namespaces.WpNamespace + name);
        }

        internal static T WordpressElement<T>(this XElement element, WordpressNamespaces namespaces, string name, Func<XElement, T> afterwork) {
            var wElement = WordpressElement(element, namespaces, name);

            if (wElement != null)
                return afterwork(wElement);

            return default(T);
        }

        internal static string HtmlWordpressElement(this XElement element, WordpressNamespaces namespaces, string name) {
            var wElement = WordpressElement(element, namespaces, name);

            if (wElement == null)
                return null;

            var data = wElement.FirstNode as XCData;

            if (data == null)
                return null;

            var value = data.Value.Trim();

            if (string.IsNullOrWhiteSpace(value))
                return null;
            
            return HttpUtility.HtmlDecode(value);
        }

        internal static XElement ContentElement(this XElement element, WordpressNamespaces namespaces, string name) {
            return element.ContentElement(namespaces, name);
        }
    }
}