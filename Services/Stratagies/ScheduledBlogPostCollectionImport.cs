using System;
using System.Collections.Generic;
using Contrib.ImportExport.InternalSchema.Post;
using Contrib.ImportExport.Models;
using Orchard.ContentManagement;
using Orchard.Events;

namespace Contrib.ImportExport.Services.Stratagies {
        public interface IScheduledBlogPostCollectionImport : IEventHandler {
            void Import(ImportSettings importSettings, ContentItem parentContentItem, ICollection<Post> posts, int batchNumber);
    }

    public class ScheduledBlogPostCollectionImport : IScheduledBlogPostCollectionImport {
        private readonly IBlogPostImportStratagy _blogPostImportStratagy;
        private readonly IContentManager _contentManager;

        public ScheduledBlogPostCollectionImport(
            IBlogPostImportStratagy blogPostImportStratagy, IContentManager contentManager) {
            _blogPostImportStratagy = blogPostImportStratagy;
            _contentManager = contentManager;
        }

        public void Import(ImportSettings importSettings, ContentItem parentContentItem, ICollection<Post> posts, int batchNumber) {

            Console.WriteLine("Started Batch Number {0}", batchNumber);
            int i = 0;
            foreach (var post in posts) {
                _blogPostImportStratagy.Import(importSettings, post, parentContentItem);

                _contentManager.Flush();
                _contentManager.Clear();
                i++;
                Console.WriteLine("Batch Number {0}, Imported record {1}", batchNumber, i);
            }
            Console.WriteLine("Finished Batch Number {0}", batchNumber);
        }
    }
}