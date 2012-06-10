using Contrib.ExternalImportExport.InternalSchema.Author;
using Contrib.ExternalImportExport.Models;
using Orchard.ContentManagement;

namespace Contrib.ExternalImportExport.Services.Stratagies {
    public class AuthorsImportStratagy : IMultipleImportStratagy {
        private readonly IUserServices _userService;

        public AuthorsImportStratagy(IUserServices userService) {
            _userService = userService;
        }

        public bool IsType(object objectToImport) {
            return objectToImport is Authors;
        }

        public ContentItem Import(ImportSettings importSettings, object objectToImport, IContent parentContent) {
            Authors authorsToImport = (Authors)objectToImport;

            foreach (var author in authorsToImport.AuthorList) {
                var user = _userService.GetUser(author.Title, author.Email, true);
            }

            return null;
        }

        public void ImportAdditionalContentItems<T>(ImportSettings importSettings, T objectToImport, IContent parentContent) {
        }
    }
}