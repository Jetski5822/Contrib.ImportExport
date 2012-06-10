using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Contrib.ImportExport.InternalSchema;
using Contrib.ImportExport.InternalSchema.Author;
using Contrib.ImportExport.InternalSchema.Category;
using Contrib.ImportExport.InternalSchema.Common;
using Contrib.ImportExport.InternalSchema.Post;
using Contrib.ImportExport.InternalSchema.Tag;

namespace Contrib.ImportExport.Providers.Wordpress {
    public class WordpressBlogAssembler : IBlogAssembler {
        public string Name {
            get { return "Wordpress"; }
        }

        public Blog Assemble(Stream stream) {
            var file = XElement.Load(stream);

            WordpressNamespaces namespaces = new WordpressNamespaces {
                ExcerptNamespace = file.GetNamespaceOfPrefix("excerpt"),
                ContentNamespace = file.GetNamespaceOfPrefix("content"),
                WfwNamespace = file.GetNamespaceOfPrefix("wfw"),
                DcNamespace = file.GetNamespaceOfPrefix("dc"),
                WpNamespace = file.GetNamespaceOfPrefix("wp")
            };

            // Get the RSS channel.
            XElement channel = file.Element("channel");

            Blog blog = CreateTopLevelBlog(namespaces, channel);

            GetCategories(blog, namespaces, channel);
            GetTags(blog, namespaces, channel);
            GetPosts(blog, namespaces, channel);

            return blog;
        }

        private Blog CreateTopLevelBlog(WordpressNamespaces namespaces, XElement channel) {
            Blog blog = new Blog();

            blog.DateCreated = DateTime.Parse(Constants.ParseRssDate(channel.Element("pubDate").Value));
			
            blog.Title = new Title();
            blog.Title.Value = channel.Element("title").Value;
			
            blog.SubTitle = new Title();
            blog.SubTitle.Value = channel.Element("description").Value;

            blog.RootURL = channel.Element(namespaces.WpNamespace + "base_blog_url").Value;
			
            blog.Authors = new Authors();
            blog.Categories = new Categories();
            blog.Tags = new Tags();
            blog.Posts = new Posts();

            return blog;
        }


        private void GetCategories(Blog blog, WordpressNamespaces namespaces, XElement channel) {

            // Loop through the elements and build the category list.
            IEnumerable<XElement> categories =
                from cat in channel.Elements(namespaces.WpNamespace + "category")
                select cat;

            foreach (XElement categoryElement in categories) {
                var createdCategory = InternalSchemaAssemblers.AssembleCategory(namespaces, categoryElement);
                if (blog.Categories.CategoryList.All(o => o.ID != createdCategory.ID))
                    blog.Categories.CategoryList.Add(createdCategory);
            }

            // WordPress stores the parent category as the description, but BlogML wants the ID.  Now that we have a
            // complete list, we'll go back through and fix them.
            IEnumerable<Category> children =
                from a in blog.Categories.CategoryList
                where a.ParentCategory != null
                select a;

            foreach (Category child in children) {
                IEnumerable<Category> parent =
                    from a in blog.Categories.CategoryList
                    where a.Title == child.ParentCategory
                    select a;

                if (parent.Any()) {
                    child.ParentCategory = parent.ElementAt(0).ID;
                }
            }
        }

        private void GetTags(Blog blog, WordpressNamespaces namespaces, XElement channel) {
            // Loop through the elements and build the category list.
            IEnumerable<XElement> tags =
                from tag in channel.Elements(namespaces.WpNamespace + "tag")
                select tag;

            foreach (XElement tagElement in tags) {
                var createdTag = InternalSchemaAssemblers.AssembleTag(namespaces, tagElement);
                if (blog.Tags.TagList.All(o => o.ID != createdTag.ID))
                    blog.Tags.TagList.Add(createdTag);
            }

            // WordPress stores the parent category as the description, but BlogML wants the ID.  Now that we have a
            // complete list, we'll go back through and fix them.
            IEnumerable<Tag> children =
                from a in blog.Tags.TagList
                where a.Slug != null
                select a;

            foreach (Tag child in children) {
                IEnumerable<Tag> parent =
                    from a in blog.Tags.TagList
                    where a.Title == child.Slug
                    select a;

                if (0 < parent.Count()) {
                    child.Slug = parent.ElementAt(0).ID;
                }
            }
        }

        private void GetPosts(Blog blog, WordpressNamespaces namespaces, XElement channel) {

            IEnumerable<XElement> posts =
                from item in channel.Elements("item")
                where item.Element(namespaces.WpNamespace + "status").Value == "publish"
                select item;

            // NGM Might want update urls within content to stop redirects?

            foreach (XElement item in posts) {

                Post post = InternalSchemaAssemblers.AssemblePost(namespaces, item);

                // We need to get the author reference separately, as we need the AuthorList from the blog.
                AuthorReference author = new AuthorReference();
                author.ID = GetAuthorReference(blog,
                                               ((XText)item.Element(namespaces.DcNamespace + "creator").FirstNode).Value);
                post.Authors.AuthorReferenceList.Add(author);

                blog.Posts.PostList.Add(post);
            }
        }

        private string GetAuthorReference(Blog blog, string author) {

            string id = Constants.Slug(author);

            IEnumerable<Author> authors =
                from a in blog.Authors.AuthorList
                where a.ID == id
                select a;

            if (!authors.Any()) {
                // Author not found - let's create them!
                Author newAuthor = new Author();
                newAuthor.ID = id;
                newAuthor.Email = id + "@" + (new Uri(blog.RootURL)).Host;
                newAuthor.Title = author;
                blog.Authors.AuthorList.Add(newAuthor);
            }

            return id;
        }
    }
}