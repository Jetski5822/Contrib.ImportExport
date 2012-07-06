using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Contrib.ImportExport.Services;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Contrib.ImportExport {
    public class Migrations : DataMigrationImpl {
        private readonly ITaxonomyImportService _taxonomyImportService;

        public Migrations(ITaxonomyImportService taxonomyImportService) {
            _taxonomyImportService = taxonomyImportService;
        }

        public int Create() {
            var categoryTaxonomyPart = _taxonomyImportService.CreateTaxonomy("Categories");
            var tagsTaxonomyPart = _taxonomyImportService.CreateTaxonomy("Tags");

            ContentDefinitionManager.AlterPartDefinition("BlogPostPart", builder => builder
                .WithField("Categories", cfg => cfg
                    .OfType("TaxonomyField")
                    .WithSetting("TaxonomyFieldSettings.AllowCustomTerms", "true")
                    .WithSetting("TaxonomyFieldSettings.Taxonomy", categoryTaxonomyPart.Name))
                .WithField("Tags", cfg => cfg
                    .OfType("TaxonomyField")
                    .WithSetting("TaxonomyFieldSettings.AllowCustomTerms", "true")
                    .WithSetting("TaxonomyFieldSettings.Taxonomy", tagsTaxonomyPart.Name)));

            return 1;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterPartDefinition("ExcerptPart", builder => builder
                .WithField("Excerpt", cfg => cfg.OfType("TextField"))
                .Attachable());

            ContentDefinitionManager.AlterTypeDefinition("BlogPost", cfg => cfg
                .WithPart("ExcerptPart"));

            return 2;
        }
    }
}