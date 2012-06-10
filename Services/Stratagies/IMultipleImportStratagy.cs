using Contrib.ImportExport.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Contrib.ImportExport.Services.Stratagies {
    public interface IMultipleImportStratagy : IDependency {
        bool IsType(object objectToImport);
        ContentItem Import(ImportSettings importSettings, object objectToImport, IContent parentContent = null);
        void ImportAdditionalContentItems<T>(ImportSettings importSettings, T objectToImport, IContent parentContent);
    }
}