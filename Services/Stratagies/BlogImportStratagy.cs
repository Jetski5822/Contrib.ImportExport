using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Contrib.ImportExport.InternalSchema;
using Contrib.ImportExport.InternalSchema.Post;
using Contrib.ImportExport.Models;
using Orchard;
using Orchard.Autoroute.Models;
using Orchard.Blogs.Models;
using Orchard.Blogs.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Title.Models;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor;
using Orchard.Environment.State;
using Orchard.Security;
using Orchard.Settings;
using Orchard.Tasks;

namespace Contrib.ImportExport.Services.Stratagies {
    public interface IBlogImportStratagy : IDependency {
        bool IsType(object objectToImport);
        ContentItem Import(ImportSettings importSettings, object objectToImport, IContent parentContent = null);
        void ImportAdditionalContentItems<T>(ImportSettings importSettings, T objectToImport, IContent parentContent);
    }

    public class BlogImportStratagy : IBlogImportStratagy {
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IMultipleImportStratagy> _importStratagies;
        private readonly IBackgroundTask _backgroundTask;
        private readonly IMembershipService _membershipService;
        private readonly ISiteService _siteService;
        private readonly IBlogService _blogService;
        private readonly IProcessingEngine _processingEngine;
        private readonly ShellSettings _shellSettings;
        private readonly IShellDescriptorManager _shellDescriptorManager;

        public BlogImportStratagy(IContentManager contentManager, 
            IEnumerable<IMultipleImportStratagy> importStratagies,
            IBackgroundTask backgroundTask,
            IMembershipService membershipService,
            ISiteService siteService,
            IBlogService blogService,
            IProcessingEngine processingEngine,
            ShellSettings shellSettings,
            IShellDescriptorManager shellDescriptorManager) {

            _contentManager = contentManager;
            _importStratagies = importStratagies;
            _backgroundTask = backgroundTask;
            _membershipService = membershipService;
            _siteService = siteService;
            _blogService = blogService;
            _processingEngine = processingEngine;
            _shellSettings = shellSettings;
            _shellDescriptorManager = shellDescriptorManager;
        }

        public bool IsType(object objectToImport) {
            return objectToImport is Blog;
        }

        public ContentItem Import(ImportSettings importSettings, object objectToImport, IContent parentContent) {
            var blogToImport = (Blog)objectToImport;

            var slug = !string.IsNullOrEmpty(importSettings.DefaultBlogSlug) ? importSettings.DefaultBlogSlug : Slugify(blogToImport.Title.Value);

            var currentBlog = _blogService.Get(slug);

            ContentItem contentItem;

            if (currentBlog != null)
                contentItem = currentBlog.ContentItem;
            else
                contentItem = _contentManager.Create("Blog", VersionOptions.Draft);

            contentItem.As<TitlePart>().Title = blogToImport.Title.Value;

            contentItem.As<BlogPart>().Description = blogToImport.SubTitle.Value;

            contentItem.As<AutoroutePart>().DisplayAlias = slug;

            _contentManager.Publish(contentItem);

            contentItem.As<ICommonPart>().CreatedUtc = blogToImport.DateCreated;
            contentItem.As<ICommonPart>().VersionCreatedUtc = blogToImport.DateCreated;
            contentItem.As<ICommonPart>().Owner = _membershipService.GetUser(_siteService.GetSiteSettings().SuperUser);

            _contentManager.Flush();

            _backgroundTask.Sweep();
            
            ImportAdditionalContentItems(importSettings, blogToImport.Authors, contentItem);

            Console.WriteLine("Blog imported");
            Console.WriteLine("Importing blog posts");

            var recordsToProcess = importSettings.RecordsToProcess == 0 ? blogToImport.Posts.PostList.Count : importSettings.RecordsToProcess;
            var blogPostsToImport = blogToImport.Posts.PostList.Skip(importSettings.StartRecordNumber).Take(recordsToProcess).ToList();

            ImportPostsInBatches(importSettings, contentItem, blogPostsToImport);

            return contentItem;
        }

        public void ImportAdditionalContentItems<T>(ImportSettings importSettings, T objectToImport, IContent parentContent) {
            foreach (var importStratagy in _importStratagies.Where(importStratagy => importStratagy.IsType(objectToImport))) {
                importStratagy.Import(importSettings, objectToImport, parentContent);
            }
        }

        private void ImportPostsInBatches(ImportSettings importSettings, IContent parentContentItem, ICollection<Post> posts) {
            _contentManager.Flush();
            _contentManager.Clear();
            const int batchSize = 100;
            int batchNo = 1;
            for (var i = 0; i < posts.Count; i += batchSize) {
                var postBatch = posts.Skip(i).Take(batchSize).ToList();
                _processingEngine.AddTask(
                    _shellSettings,
                    _shellDescriptorManager.GetShellDescriptor(),
                    "IScheduledBlogPostCollectionImport.Import",
                    new Dictionary<string, object> {
                        {"importSettings", importSettings},
                        {"parentContentItem", parentContentItem},
                        {"posts", postBatch},
                        {"batchNumber", batchNo}
                    });

                batchNo++;
            }
        }

        private static string Slugify(string slug) {
            var dissallowed = new Regex(@"[/:?#\[\]@!$&'()*+,;=\s]+");

            slug = dissallowed.Replace(slug, "-");
            slug = slug.Trim('-');

            if (slug.Length > 1000)
                slug = slug.Substring(0, 1000);

            return slug.ToLowerInvariant();
        }
    }
}