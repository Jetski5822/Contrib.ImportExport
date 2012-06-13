using System.Collections.Generic;
using Contrib.ImportExport.Models;

namespace Contrib.ImportExport.ViewModels {
    public class ImportAdminViewModel {
        public ImportSettings Settings { get; set; }

        public IEnumerable<KeyValuePair<int, string>> Blogs { get; set; }
    }
}