using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using Contrib.ImportExport.InternalSchema;
using Contrib.ImportExport.Models;
using Contrib.ImportExport.Providers;
using Contrib.ImportExport.Services.Strategies;
using ICSharpCode.SharpZipLib.Zip;
using JetBrains.Annotations;
using Orchard;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.Tasks;
using Orchard.UI.Notify;

namespace Contrib.ImportExport.Services {
    public class ImportService : IImportService {

        private readonly IBackgroundTask _backgroundTask;
        private readonly IBlogImportStrategy _blogImportStrategy;
        private readonly IEnumerable<IBlogAssembler> _blogAssemblers;
        private readonly IOrchardServices _orchardServices;

        public ImportService(IOrchardServices orchardServices,
            IBackgroundTask backgroundTask,
            IBlogImportStrategy blogImportStrategy,
            IEnumerable<IBlogAssembler> blogAssemblers) {
            _backgroundTask = backgroundTask;
            _blogImportStrategy = blogImportStrategy;
            _blogAssemblers = blogAssemblers;
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
        }

        protected virtual ISite CurrentSite { get; [UsedImplicitly] private set; }
        public Localizer T { get; set; }

        #region IImportService Members

        public void Import(HttpPostedFileBase httpPostedFileBase, ImportSettings importSettings) {
            if (importSettings == null)
                throw new ArgumentNullException("importSettings");

            if (httpPostedFileBase.FileName.EndsWith(".zip")) {
                var blogs = UnzipMediaFileArchiveToBlogMLBlog(httpPostedFileBase, importSettings);
                foreach (var blog in blogs) {
                    ImportBlog(blog, importSettings);
                }
            }
            else if ((httpPostedFileBase.FileName.EndsWith(".xml"))) {
                ImportStream(importSettings, httpPostedFileBase.InputStream);
            }
        }

        public void Import(string urlItemPath, ImportSettings importSettings) {
            var client = new WebClient();
            Stream stream = null;
            try {
                stream = client.OpenRead(urlItemPath);

                ImportStream(importSettings, stream);
            }
            catch (Exception ex) {
                _orchardServices.Notifier.Error(T("An error occured loading your blog, please check permissions and file is accessible, error: {0}", ex.Message));
            } finally {
                if (stream != null)
                    stream.Close();
                client.Dispose();
            }
        }

        private void ImportStream(ImportSettings importSettings, Stream stream) {
            try {
                var blog = AssembleBlog(stream, importSettings);
                ImportBlog(blog, importSettings);
            }
            catch {
                _orchardServices.Notifier.Error(T("An error occured importing your blog"));
            }
        }

        private Blog AssembleBlog(Stream stream, ImportSettings importSettings) {
            return _blogAssemblers.Single(o => o.Name == importSettings.SelectedSchema).Assemble(stream);
        }

        private void ImportBlog(Blog blog, ImportSettings importSettings) {
            _blogImportStrategy.Import(importSettings, blog, null);

            RebuildOrchardIndexes();

            _orchardServices.Notifier.Information(T("Blog Import has completed successfully. All errors will be listed below, and you may review report generated via the reports link in the menu at anytime."));
        }

        #endregion

        private void RebuildOrchardIndexes() {
            _backgroundTask.Sweep();
        }

        private IEnumerable<Blog> UnzipMediaFileArchiveToBlogMLBlog(HttpPostedFileBase postedFile, ImportSettings importSettings) {
            var postedFileLength = postedFile.ContentLength;
            var postedFileStream = postedFile.InputStream;
            var postedFileData = new byte[postedFileLength];
            postedFileStream.Read(postedFileData, 0, postedFileLength);

            var blogMlBlogs = new List<Blog>();

            using (var memoryStream = new MemoryStream(postedFileData)) {
                var fileInflater = new ZipInputStream(memoryStream);
                ZipEntry entry;
                while ((entry = fileInflater.GetNextEntry()) != null) {
                    if (entry.IsDirectory || entry.Name.Length <= 0)
                        continue;

                    if (entry.IsFile && entry.Name.EndsWith(".xml"))
                        blogMlBlogs.Add(AssembleBlog(fileInflater, importSettings));
                }
            }

            return blogMlBlogs.ToArray();
        }
    }
}