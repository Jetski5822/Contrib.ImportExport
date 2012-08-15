using System;
using System.Linq;
using System.Text.RegularExpressions;
using Contrib.ImportExport.Models;
using HtmlAgilityPack;
using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Logging;

namespace Contrib.ImportExport.Services {
    public interface IDataCleaner : IDependency {
        string Clean(string value, ImportSettings importSettings);
    }

    public class DataCleaner : IDataCleaner {
        private readonly IStorageProvider _storageProvider;

        public DataCleaner(IStorageProvider storageProvider) {
            _storageProvider = storageProvider;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public string Clean(string value, ImportSettings importSettings) {
            if (string.IsNullOrEmpty(value))
                return value;

            try {
                value = UpdateImgLocations(value, importSettings);

                value = RemoveEmptyWhiteSpace(value, "span,p,b");

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(value);

                RemoveComments(htmlDocument);
                RemoveMicrosoftCrap(htmlDocument);

                //UpdateUrlTagLocations("title", htmlDocument, importSettings);
                UpdateUrlTagLocations("href", htmlDocument, importSettings);

                RemoveTags(htmlDocument, "br");

                value = htmlDocument.DocumentNode.OuterHtml;

                value = RemoveEmptyWhiteSpace(value, "span,p,b");

                return value;
            } catch (Exception exception) {
                Console.WriteLine(exception.ToString());
            }
            return value;
        }

        public void RemoveComments(HtmlDocument htmlDocument) {
            var nodes = htmlDocument.DocumentNode.SelectNodes("//comment()");
            if (nodes != null) {
                foreach (HtmlNode comment in nodes) {
                    comment.ParentNode.RemoveChild(comment);
                }
            }
        }

        public void RemoveTags(HtmlDocument html, string tagName) {
            var tags = html.DocumentNode.SelectNodes("//" + tagName);
            if (tags != null) {
                foreach (var tag in tags) {
                    if (!tag.HasChildNodes) {
                        tag.ParentNode.RemoveChild(tag);
                        continue;
                    }

                    for (var i = tag.ChildNodes.Count - 1; i >= 0; i--) {
                        var child = tag.ChildNodes[i];
                        tag.ParentNode.InsertAfter(child, tag);
                    }
                    tag.ParentNode.RemoveChild(tag);
                }
            }
        }

        private void UpdateUrlTagLocations(string attribute, HtmlDocument html, ImportSettings importSettings) {
            var anchorTags = html.DocumentNode.SelectNodes("//a[@" + attribute + "]");
            if (anchorTags == null)
                return;

            foreach (HtmlNode link in anchorTags) {
                try {
                    HtmlAttribute att = link.Attributes[attribute];
                    var value = att.Value;

                    if (string.IsNullOrWhiteSpace(value))
                        continue;

                    Uri uriPreFormatted;
                    if (!Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out uriPreFormatted)) {
                        //Logger.Debug("Could not create Uri");
                        continue;
                    }

                    if (!uriPreFormatted.IsAbsoluteUri) {
                        //var absolutePublicUrl = _storageProvider.GetPublicUrl(uriPreFormatted.OriginalString.Replace("\r\n", string.Empty).TrimStart('/'));

                        //att.Value = value.Replace(value, absolutePublicUrl);

                        //Logger.Debug(string.Format("Converted absolute Url To: {0}", absolutePublicUrl));

                        //Logger.Debug(string.Format("Is not an absolute path: {0}", uriPreFormatted.OriginalString));
                        continue;
                    }

                    if (!IsHostCorrect(uriPreFormatted, importSettings)) {
                        //Logger.Debug(string.Format("Host Not in offending list: {0}", uriPreFormatted.Host));
                        continue;
                    }

                    att.Value = uriPreFormatted.AbsolutePath;//.TrimStart('/');

                    //Logger.Debug(string.Format("Converted Url To: {0}", absoluteUri));
                } catch (Exception ex) {
                    Logger.Debug("Error converting URI, exception: " + ex.ToString());
                }
            }
        }

        private string UpdateImgLocations(string value, ImportSettings importSettings) {
            if (string.IsNullOrEmpty(value))
                return value;

            var matches = Regex.Matches(value, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.Singleline);

            if (matches.Count == 0)
                return value;

            foreach (Match match in matches) {
                var urlPreFormatted = 
                    match.Value.Substring(match.Value.IndexOf("src=\"") + 5, (match.Value.IndexOf("\"", match.Value.IndexOf("src=\"") + 6)) - match.Value.IndexOf("src=\"") - 5);

                Logger.Debug(string.Format("Url Before: {0}", urlPreFormatted));

                Uri uriPreFormatted;
                if (!Uri.TryCreate(urlPreFormatted, UriKind.RelativeOrAbsolute, out uriPreFormatted)) {
                    Logger.Debug("Could not create Uri");
                    continue;
                }

                if (!uriPreFormatted.IsAbsoluteUri) {
                    var absolutePublicUrl = _storageProvider.GetPublicUrl(uriPreFormatted.OriginalString.Replace("\r\n", string.Empty).TrimStart('/'));

                    value = value.Replace(urlPreFormatted, absolutePublicUrl);

                    Logger.Debug(string.Format("Converted absolute Url To: {0}", absolutePublicUrl));

                    Logger.Debug(string.Format("Is not an absolute path: {0}", uriPreFormatted.OriginalString));
                    continue;
                }

                if (!IsHostCorrect(uriPreFormatted, importSettings)) {
                    Logger.Debug(string.Format("Host Not in offending list: {0}", uriPreFormatted.Host));
                    continue;
                }

                var publicUrl = _storageProvider.GetPublicUrl(uriPreFormatted.AbsolutePath.TrimStart('/'));

                var formattedUrl = uriPreFormatted.GetLeftPart(UriPartial.Authority) + publicUrl;

                // New
                var absoluteUri = new Uri(formattedUrl).AbsolutePath;

                value = value.Replace(urlPreFormatted, absoluteUri);

                Logger.Debug(string.Format("Converted Url To: {0}", absoluteUri));

                // Old
                //value = value.Replace(urlPreFormatted, formattedUrl);

                //value = CleanHost(value);

                //Console.WriteLine(string.Format("Url After: {0}", formattedUrl));
            }

            return value;
        }

        private string RemoveEmptyWhiteSpace(string value, string name) {
            var valuesToSplitOn = name.Split(',');

            foreach (var splitValue in valuesToSplitOn) {
                var pattern = string.Format(@"<{0}[^>]*(?:/>|>(?:\s)*</{0}>)", splitValue);
                bool isThereMatches = Regex.IsMatch(value, pattern, RegexOptions.Singleline);

                while (isThereMatches) {
                    value = Regex.Replace(value, pattern, string.Empty, RegexOptions.Singleline);

                    isThereMatches = Regex.IsMatch(value, pattern, RegexOptions.Singleline);
                }
            }

            foreach (var splitValue in valuesToSplitOn) {
                var pattern = string.Format(@"<{0}[^>]*(?:/>|>(?:\s)*</{0}>)", splitValue);
                bool isThereMatches = Regex.IsMatch(value, pattern, RegexOptions.Singleline);

                if (isThereMatches)
                    RemoveEmptyWhiteSpace(value, name);
            }

            return value;
        }

        private void RemoveMicrosoftCrap(HtmlDocument htmlDocument) {

            var unknown = htmlDocument.DocumentNode.DescendantsAndSelf().Where(
                node => node.NodeType == HtmlNodeType.Element && node.Name.StartsWith("o:"));

            foreach (var node in unknown.ToArray()) {
                var parent = node.ParentNode;
                var siblings = parent.ChildNodes.ToArray();
                var children = node.ChildNodes.ToArray();
                node.RemoveAllChildren();
                parent.RemoveAllChildren();
                var newSiblings = siblings.SelectMany(sibling => sibling == node ? children : new[] { sibling });
                foreach (var newSibling in newSiblings)
                    parent.AppendChild(newSibling);
            }
        }


        private static bool IsHostCorrect(Uri uriPreFormatted, ImportSettings importSettings) {
            if (uriPreFormatted.Scheme == "mailto")
                return true;

            if (importSettings.OffendingHosts == null)
                return true;

            return importSettings.OffendingHosts.Contains(uriPreFormatted.Host);
        }
    }
}