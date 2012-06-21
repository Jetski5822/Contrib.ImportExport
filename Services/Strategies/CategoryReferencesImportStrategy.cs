using System;
using System.Collections.Concurrent;
using System.Linq;
using Contrib.ImportExport.InternalSchema.Post;
using Contrib.ImportExport.Models;
using Contrib.Taxonomies.Models;
using Contrib.Taxonomies.Services;
using Orchard.ContentManagement;

namespace Contrib.ImportExport.Services.Strategies {
    public class CategoryReferencesImportStrategy : IMultipleImportStrategy {
        private readonly ITaxonomyImportService _taxonomyImportService;
        private readonly ITaxonomyService _taxonomyService;
        private readonly IContentManager _contentManager;

        private readonly ConcurrentDictionary<string, TermPart> _categories = new ConcurrentDictionary<string, TermPart>(); 

        public CategoryReferencesImportStrategy(ITaxonomyImportService taxonomyImportService, ITaxonomyService taxonomyService, IContentManager contentManager) {
            _taxonomyImportService = taxonomyImportService;
            _taxonomyService = taxonomyService;
            _contentManager = contentManager;
        }

        public bool IsType(object objectToImport) {
            return objectToImport is CategoryReferences;
        }

        public ContentItem Import(ImportSettings importSettings, object objectToImport, IContent parentContent) {
            CategoryReferences categoriesToImport = (CategoryReferences)objectToImport;

            if (categoriesToImport.CategoryReferenceList.Count == 0)
                return null;

            const string taxonomyName = "Categories";
            var taxonomyPart = _taxonomyImportService.CreateTaxonomy(taxonomyName);

            var currentTerms = _taxonomyService.GetTermsForContentItem(parentContent.ContentItem.Id, taxonomyName).ToList();

            foreach (var categoryReference in categoriesToImport.CategoryReferenceList) {
                if (string.IsNullOrWhiteSpace(categoryReference.Title))
                    continue;

                var term = _taxonomyService.GetTermByName(taxonomyPart.Id, categoryReference.Title);

                if (term == null) {
                    CategoryReference reference = categoryReference;

                    term = _categories.GetOrAdd(reference.ID, (o) => _taxonomyImportService.CreateTermFor(taxonomyPart, reference.Title, reference.ID));
                }

                var currentTerm = currentTerms.FirstOrDefault(o => o.Id == term.Id);

                if (currentTerm == null) {
                    currentTerms.Add(term);
                }
            }

            _taxonomyService.UpdateTerms(parentContent.ContentItem, currentTerms, taxonomyName);

            return null;
        }

        public void ImportAdditionalContentItems<T>(ImportSettings importSettings, T objectToImport, IContent parentContent) {
            throw new NotSupportedException();
        }
    }
}