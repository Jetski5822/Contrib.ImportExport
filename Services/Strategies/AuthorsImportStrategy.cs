using Contrib.ImportExport.InternalSchema.Author;
using Contrib.ImportExport.Models;
using Orchard.ContentManagement;

namespace Contrib.ImportExport.Services.Strategies {
    public class AuthorsImportStrategy : IMultipleImportStrategy {
        private readonly IUserServices _userService;

        public AuthorsImportStrategy(IUserServices userService) {
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