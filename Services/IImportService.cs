using System.IO;
using System.Web;
using Contrib.ImportExport.Models;
using Orchard;

namespace Contrib.ImportExport.Services {
    public interface IImportService : IDependency {
        void Import(HttpPostedFileBase httpPostedFileBase, ImportSettings importSettings);

        void Import(string urlItemPath, ImportSettings importSettings);

        void Import(FileInfo fileInfo, ImportSettings importSettings);
    }
}
