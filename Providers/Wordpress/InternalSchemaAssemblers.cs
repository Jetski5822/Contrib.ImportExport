using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Contrib.ImportExport.InternalSchema.Category;
using Contrib.ImportExport.InternalSchema.Common;
using Contrib.ImportExport.InternalSchema.Post;
using Contrib.ImportExport.InternalSchema.Post.Additional;
using Contrib.ImportExport.InternalSchema.Tag;

namespace Contrib.ImportExport.Providers.Wordpress {
    internal static class InternalSchemaAssemblers {
        internal static Category AssembleCategory(WordpressNamespaces namespaces, XElement categoryElement) {
            var category = new Category();

            category.ID =
                categoryElement.WordpressElement(namespaces, "category_nicename", (e) => e.Value.Trim());

            category.Description = 
                categoryElement.WordpressElement(namespaces, "category_description", (e) =>((XCData)e.FirstNode).Value.Trim());

            category.Title =
                categoryElement.HtmlWordpressElement(namespaces, "cat_name");

            category.ParentCategory = 
                categoryElement.WordpressElement(namespaces, "category_parent", (e) => e.Value.Trim());

            return category;
        }

        internal static Tag AssembleTag(WordpressNamespaces namespaces, XElement tagElement) {
            var tag = new Tag();

            tag.ID =
                tagElement.WordpressElement(namespaces, "tag_slug", (e) => e.Value.Trim());

            tag.Slug =
                tagElement.WordpressElement(namespaces, "tag_slug", (e) => e.Value.Trim());

            tag.Title = 
                tagElement.HtmlWordpressElement(namespaces, "tag_name");

            return tag;
        }

        internal static Post AssemblePost(WordpressNamespaces namespaces, XElement postElement) {
			Post post = new Post();

			// Node (parent) properties.
            post.ID = postElement.WordpressElement(namespaces, "post_id").Value;
            post.Title = postElement.Element("title").Value;
            post.DateCreated = DateTime.Parse(Constants.ParseRssDate(postElement.Element("pubDate").Value));
            post.DateModified = DateTime.Parse(Constants.ParseRssDate(postElement.Element("pubDate").Value));
            post.PostUrl = (new Uri(postElement.Element("link").Value).AbsolutePath).TrimStart('/'); // NGM
            post.Approved = true;

			// Object properties.
            post.Content = new Content();
            post.Content.Type = Content.TypeHTML;
            post.Content.Value = ((XCData)postElement.ContentElement(namespaces, "encoded").FirstNode).Value;

            post.PostName = new Title();
            post.PostName.Type = Content.TypeHTML;
            post.PostName.Value = postElement.Element("title").Value;

            string excerpt = ((XCData)postElement.Element(namespaces.ExcerptNamespace + "encoded").FirstNode).Value;
			if (String.Empty == excerpt) {
                post.HasExcerpt = false;
			}
			else {
                post.Excerpt = new Content();
                post.Excerpt.Type = Content.TypeHTML;
                post.Excerpt.Value = excerpt;
                post.HasExcerpt = true;
			}
			
			// Category references.
			IEnumerable<XElement> categories =
				from cat in postElement.Elements("category")
				where cat.Attribute("domain") != null && cat.Attribute("domain").Value == "category"
				select cat;
			
			if (categories.Any()) {
                post.Categories = new CategoryReferences();
				foreach(XElement reference in categories) {
                    post.Categories.CategoryReferenceList.Add(AssembleCategoryReference(reference));
				}
            }

            // Tag references.
            IEnumerable<XElement> tags =
                from tag in postElement.Elements("category")
                where tag.Attribute("domain") != null && (tag.Attribute("domain").Value == "post_tag" || tag.Attribute("domain").Value == "tag") && tag.Attribute("nicename") != null
                select tag;

            if (tags.Any()) {
                post.Tags = new TagReferences();
                foreach (XElement reference in tags) {
                    post.Tags.TagReferenceList.Add(new TagReference(reference));
                }
            }
			
			// Comments on this post.
            IEnumerable<XElement> comments =
                from comment in postElement.Elements(namespaces.WpNamespace + "comment")
                where comment.WordpressElement(namespaces, "comment_approved").Value == "1"
                      && string.IsNullOrEmpty(comment.WordpressElement(namespaces, "comment_type").Value)
				select comment;

            post.Comments = new Comments();

			if (comments.Any()) {
				foreach(XElement comment in comments) {
                    post.Comments.CommentList.Add(AssembleComment(namespaces, comment));
				}
			}

            if (postElement.WordpressElement(namespaces, "comment_status").Value == "open")
                post.Comments.Enabled = true;
            else {
                post.Comments.Enabled = false;
            }

			// Trackbacks for this post.
			IEnumerable<XElement> trackbax =
				from tb in postElement.Elements(namespaces.WpNamespace + "comment")
				where tb.WordpressElement(namespaces, "comment_approved").Value == "1"
					&& ((tb.WordpressElement(namespaces, "comment_type").Value == "trackback")
					    || (tb.WordpressElement(namespaces, "comment_type").Value == "pingback"))
				select tb;
			
			if (trackbax.Any()) {
				post.Trackbacks = new Trackbacks();
				foreach(XElement trackback in trackbax) {
                    post.Trackbacks.TrackbackList.Add(AssembleTrackback(namespaces, trackback));
				}
			}

            post.Authors = new AuthorReferences();
            post.Authors.AuthorReferenceList.Add(new AuthorReference { ID = postElement.Element(namespaces.DcNamespace + "creator").Value });

		    return post;
		}

        internal static CategoryReference AssembleCategoryReference(XElement referenceElement) {
            CategoryReference reference = new CategoryReference();

            reference.ID = referenceElement.Attribute("nicename").Value.Trim();
            reference.Title = referenceElement.Value.Trim();

            return reference;
        }

        internal static Comment AssembleComment(WordpressNamespaces namespaces, XElement commentElement) {
            Comment comment = new Comment();

            // Node (parent) properties.
            comment.ID = commentElement.WordpressElement(namespaces, "comment_id").Value;
            comment.Title = comment.ID;
            comment.DateCreated = DateTime.Parse(commentElement.WordpressElement(namespaces, "comment_date_gmt").Value);

            comment.Content = new Content();
            comment.Content.Type = Content.TypeHTML;
            comment.Content.Value = ((XCData)commentElement.WordpressElement(namespaces, "comment_content").FirstNode).Value;

            comment.UserName = ((XCData)commentElement.WordpressElement(namespaces, "comment_author").FirstNode).Value;

            string email = commentElement.WordpressElement(namespaces, "comment_author_email").Value;
            if (!string.IsNullOrWhiteSpace(email)) {
                comment.UserEmail = email;
            }

            string url = commentElement.WordpressElement(namespaces, "comment_author_url").Value;
            if (!string.IsNullOrWhiteSpace(url)) {
                comment.UserURL = url;
            }

            string approved = commentElement.WordpressElement(namespaces, "comment_approved").Value;
            if (!string.IsNullOrWhiteSpace(approved)) {
                comment.Approved = approved == "1";
            }

            return comment;
        }

        internal static Trackback AssembleTrackback(WordpressNamespaces namespaces, XElement trackbackElement) {
            Trackback trackback = new Trackback();

            trackback.ID = trackbackElement.WordpressElement(namespaces, "comment_id").Value;
            trackback.Title = ((XCData)trackbackElement.WordpressElement(namespaces, "comment_author").FirstNode).Value;
            trackback.DateCreated =
                DateTime.Parse(trackbackElement.WordpressElement(namespaces, "comment_date_gmt").Value);

            trackback.Url = trackbackElement.WordpressElement(namespaces, "comment_author_url").Value;

            return trackback;
        }
    }
}