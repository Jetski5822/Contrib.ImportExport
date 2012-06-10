using Contrib.ExternalImportExport.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Contrib.ExternalImportExport.Services.Stratagies {
    public interface IBlogPostImportStratagy : IDependency {
        bool IsType(object objectToImport);
        ContentItem Import(ImportSettings importSettings, object objectToImport, IContent parentContent = null);
        void ImportAdditionalContentItems<T>(ImportSettings importSettings, T objectToImport, IContent parentContent);
    }
}