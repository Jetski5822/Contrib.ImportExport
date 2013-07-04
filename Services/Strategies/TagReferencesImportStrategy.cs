using System;
using System.Collections.Concurrent;
using System.Linq;
using Contrib.ImportExport.InternalSchema.Post.Additional;
using Contrib.ImportExport.Models;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
using Orchard.ContentManagement;

namespace Contrib.ImportExport.Services.Strategies {
    public class TagReferencesImportStrategy : IMultipleImportStrategy {
        private readonly ITaxonomyImportService _taxonomyImportService;
        private readonly ITaxonomyService _taxonomyService;
        private readonly IContentManager _contentManager;

        private readonly ConcurrentDictionary<string, TermPart> _tags = new ConcurrentDictionary<string, TermPart>(); 

        public TagReferencesImportStrategy(ITaxonomyImportService taxonomyImportService, ITaxonomyService taxonomyService, IContentManager contentManager) {
            _taxonomyImportService = taxonomyImportService;
            _taxonomyService = taxonomyService;
            _contentManager = contentManager;
        }

        public bool IsType(object objectToImport) {
            return objectToImport is TagReferences;
        }

        public ContentItem Import(ImportSettings importSettings, object objectToImport, IContent parentContent) {
            TagReferences tagsToImport = (TagReferences)objectToImport;

            if (tagsToImport.TagReferenceList.Count == 0)
                return null;

            var taxonomyPart = _taxonomyImportService.CreateTaxonomy("Tags");

            var currentTerms = _taxonomyService.GetTermsForContentItem(parentContent.ContentItem.Id, "Tags").ToList();

            foreach (var tagReference in tagsToImport.TagReferenceList) {
                if (string.IsNullOrWhiteSpace(tagReference.Title))
                    continue;

                var term = _taxonomyService.GetTermByName(taxonomyPart.Id, tagReference.Title);
                
                if (term == null) {
                    TagReference reference = tagReference;

                    term = _tags.GetOrAdd(reference.ID, (o) => _taxonomyImportService.CreateTermFor(taxonomyPart, reference.Title, reference.ID));
                }

                var currentTerm = currentTerms.FirstOrDefault(o => o.Id == term.Id);

                if (currentTerm == null) {
                    currentTerms.Add(term);
                }
            }

            _taxonomyService.UpdateTerms(parentContent.ContentItem, currentTerms, "Tags");

            return null;
        }

        public void ImportAdditionalContentItems<T>(ImportSettings importSettings, T objectToImport, IContent parentContent) {
            throw new NotSupportedException();
        }
    }
}