using System;
using System.Collections.Generic;
using Contrib.ImportExport.InternalSchema.Post;
using Contrib.ImportExport.Models;
using Orchard.ContentManagement;
using Orchard.Events;

namespace Contrib.ImportExport.Services.Stratagies {
        public interface IScheduledBlogPostCollectionImport : IEventHandler {
        void Import(ImportSettings importSettings, ContentItem parentContentItem, ICollection<Post> blogPosts, int batchNumber);
    }

    public class ScheduledBlogPostCollectionImport : IScheduledBlogPostCollectionImport {
        private readonly IBlogPostImportStratagy _blogPostImportStratagy;
        private readonly IContentManager _contentManager;

        public ScheduledBlogPostCollectionImport(
            IBlogPostImportStratagy blogPostImportStratagy, IContentManager contentManager) {
            _blogPostImportStratagy = blogPostImportStratagy;
            _contentManager = contentManager;
        }

        public void Import(ImportSettings importSettings, ContentItem parentContentItem, ICollection<Post> blogPosts, int batchNumber) {

            int i = 0;
            foreach (var blogPost in blogPosts) {
                _blogPostImportStratagy.Import(importSettings, blogPost, parentContentItem);

                _contentManager.Flush();
                _contentManager.Clear();
                i++;
                Console.WriteLine("Batch Number {0}, Imported record {1}", batchNumber, i);
            }
        }
    }
}