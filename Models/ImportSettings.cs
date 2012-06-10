namespace Contrib.ExternalImportExport.Models {
    public class ImportSettings {
        public string UrlItemPath { get; set; }

        public string SlugPattern { get; set; }

        public string DefaultBlogSlug { get; set; }

        public int BlogIdToImportInto { get; set; }

        public string[] OffendingHosts { get; set; }

        public bool Override { get; set; }

        public int StartRecordNumber { get; set; }

        public int RecordsToProcess { get; set; }
    }
}
