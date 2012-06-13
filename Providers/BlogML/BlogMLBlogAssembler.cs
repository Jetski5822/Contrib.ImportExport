using System;
using System.IO;
using BlogML.Xml;
using Contrib.ImportExport.InternalSchema;
using Contrib.ImportExport.InternalSchema.Author;
using Contrib.ImportExport.InternalSchema.Category;
using Contrib.ImportExport.InternalSchema.Common;
using Contrib.ImportExport.InternalSchema.Post;
using Contrib.ImportExport.InternalSchema.Tag;
using JetBrains.Annotations;
using Orchard;
using Orchard.Localization;
using Orchard.UI.Notify;

namespace Contrib.ImportExport.Providers.BlogML {
    public class BlogMLBlogAssembler : IBlogAssembler {
        private readonly IOrchardServices _orchardServices;

        public BlogMLBlogAssembler(IOrchardServices orchardServices) {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public string Name {
            get { return "BlogML"; }
        }

        public Blog Assemble(Stream stream) {
            var blogMLBlog = DeserializeBlogMlByStream(stream);

            if (blogMLBlog == null) {
                _orchardServices.Notifier.Information(T("No blog to import"));
                return null;
            }

            Blog blog = CreateTopLevelBlog(blogMLBlog);

            GetCategories(blog, blogMLBlog);
            GetAuthors(blog, blogMLBlog);
            GetPosts(blog, blogMLBlog);

            return blog;
        }

        private void GetPosts(Blog blog, BlogMLBlog blogMLBlog) {
            foreach (var blogMLPost in blogMLBlog.Posts) {
                Post post = new Post();
                post.ID = blogMLPost.ID;

                foreach (BlogMLAttachment blogMLAttachment in blogMLPost.Attachments) {
                    Attachment attachment = new Attachment();
                    attachment.Embedded = blogMLAttachment.Embedded;
                    attachment.Value = blogMLAttachment.Data;
                    attachment.MimeType = blogMLAttachment.MimeType;
                    attachment.ExternalURI = blogMLAttachment.Path;
                    attachment.URL = blogMLAttachment.Url;
                    post.Attachments.AttachmentList.Add(attachment);
                }

                foreach (BlogMLAuthorReference blogMLAuthor in blogMLPost.Authors) {
                    AuthorReference authorReference = new AuthorReference();
                    authorReference.ID = blogMLAuthor.Ref;
                    post.Authors.AuthorReferenceList.Add(authorReference);
                }

                foreach (BlogMLCategoryReference blogMLCategory in blogMLPost.Categories) {
                    CategoryReference categoryReference = new CategoryReference();
                    categoryReference.ID = blogMLCategory.Ref;
                    post.Categories.CategoryReferenceList.Add(categoryReference);
                }

                foreach (BlogMLComment blogMLComment in blogMLPost.Comments) {
                    Comment comment = new Comment();
                    comment.ID = blogMLComment.ID;
                    comment.Approved = blogMLComment.Approved;
                    comment.Content = new Content { Type = blogMLComment.Content.ContentType.ToString(), Value = blogMLComment.Content.Text };
                    comment.DateCreated = blogMLComment.DateCreated;
                    comment.DateModified = blogMLComment.DateModified;
                    comment.Title = blogMLComment.Title;
                    comment.UserEmail = blogMLComment.UserEMail;
                    comment.UserName = blogMLComment.UserName;
                    comment.UserURL = blogMLComment.UserUrl;
                    post.Comments.CommentList.Add(comment);
                }

                foreach (BlogMLTrackback blogMLTrackback in blogMLPost.Trackbacks) {
                    Trackback trackback = new Trackback();
                    trackback.ID = blogMLTrackback.ID;
                    trackback.Approved = blogMLTrackback.Approved;
                    trackback.DateCreated = blogMLTrackback.DateCreated;
                    trackback.DateModified = blogMLTrackback.DateModified;
                    trackback.Title = blogMLTrackback.Title;
                    trackback.Url = blogMLTrackback.Url;
                }

                post.Approved = blogMLPost.Approved;

                post.Content = new Content
                                   {Type = blogMLPost.Content.ContentType.ToString(), Value = blogMLPost.Content.Text};

                post.DateCreated = blogMLPost.DateCreated;
                post.DateModified = blogMLPost.DateModified;
                post.HasExcerpt = blogMLPost.HasExcerpt;

                if (post.HasExcerpt)
                    post.Excerpt = new Content
                                   {Type = blogMLPost.Excerpt.ContentType.ToString(), Value = blogMLPost.Excerpt.Text};

                post.PostName = new Title {Type = Content.TypeHTML, Value = blogMLPost.PostName};
                post.PostUrl = blogMLPost.PostUrl;
                post.Title = blogMLPost.Title;
                post.Type = blogMLPost.PostType.ToString();
                post.Views = blogMLPost.Views;
                blog.Posts.PostList.Add(post);
            }
        }

        private void GetAuthors(Blog blog, BlogMLBlog blogMLBlog) {
            foreach (var blogMLAuthor in blogMLBlog.Authors) {
                Author author = new Author();
                author.ID = blogMLAuthor.ID;
                author.Approved = blogMLAuthor.Approved;
                author.DateCreated = blogMLAuthor.DateCreated;
                author.DateModified = blogMLAuthor.DateModified;
                author.Email = blogMLAuthor.Email;
                author.Title = blogMLAuthor.Title;
                blog.Authors.AuthorList.Add(author);
            }
        }

        private void GetCategories(Blog blog, BlogMLBlog blogMLBlog) {
            foreach (var blogMLcategory in blogMLBlog.Categories) {
                Category category = new Category();
                category.ID = blogMLcategory.ID;
                category.Approved = blogMLcategory.Approved;
                category.DateCreated = blogMLcategory.DateCreated;
                category.DateModified = blogMLcategory.DateModified;
                category.Description = blogMLcategory.Description;
                category.ParentCategory = blogMLcategory.ParentRef;
                category.Title = blogMLcategory.Title;

                blog.Categories.CategoryList.Add(category);
            }
        }

        private Blog CreateTopLevelBlog(BlogMLBlog blogMLBlog) {
            Blog blog = new Blog();

            blog.DateCreated = blogMLBlog.DateCreated;

            blog.Title = new Title();
            blog.Title.Value = blogMLBlog.Title;

            blog.SubTitle = new Title();
            blog.SubTitle.Value = blogMLBlog.SubTitle;

            blog.RootURL = blogMLBlog.RootUrl;

            blog.Authors = new Authors();
            blog.Categories = new Categories();
            blog.Tags = new Tags();
            blog.Posts = new Posts();

            return blog;
        }

        [CanBeNull]
        private BlogMLBlog DeserializeBlogMlByStream(Stream stream) {
            try {
                return BlogMLSerializer.Deserialize(stream);
            }
            catch (Exception ex) {
                _orchardServices.Notifier.Error(T("Error deserializing your blog Error:{0} - Please verify that this is an XML file stream", ex.Message));
                throw;
            }
        }
    }
}