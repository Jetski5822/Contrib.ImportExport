using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Contrib.ImportExport.Models;
using Contrib.ImportExport.Services;
using Orchard.Commands;

namespace Contrib.ImportExport.Commands {
    public class ImportCommands : DefaultOrchardCommandHandler {
        private readonly IImportService _importService;

        public ImportCommands(IImportService importService) {
            _importService = importService;
        }

        [OrchardSwitch]
        public string Filename { get; set; }

        [OrchardSwitch]
        public string DefaultBlogSlug { get; set; }

        [OrchardSwitch]
        public string OffendingHosts { get; set; }

        [OrchardSwitch]
        public string Override { get; set; }

        [OrchardSwitch]
        public string StartRecordNumber { get; set; }

        [OrchardSwitch]
        public string RecordsToProcess { get; set; }

        [CommandName("wordpress import")]
        [CommandHelp("wordpress import /Filename:<Filename>, /DefaultBlogSlug:<DefaultBlogSlug> /OffendingHosts:<OffendingHosts> /Override:<Override> /StartRecordNumber:<StartRecordNumber> /RecordsToProcess:<RecordsToProcess>")]
        [OrchardSwitches("Filename,DefaultBlogSlug,OffendingHosts,Override,StartRecordNumber,RecordsToProcess")]
        public void Import() {
            var importSettings = new ImportSettings();

            if (!string.IsNullOrEmpty(DefaultBlogSlug))
                importSettings.DefaultBlogSlug = DefaultBlogSlug;

            if (!string.IsNullOrEmpty(OffendingHosts))
                importSettings.OffendingHosts = OffendingHosts.Split(',');

            if (!string.IsNullOrEmpty(Override))
                importSettings.Override = Boolean.Parse(Override);

            if (!string.IsNullOrEmpty(StartRecordNumber))
                importSettings.StartRecordNumber = Int32.Parse(StartRecordNumber);

            if (!string.IsNullOrEmpty(RecordsToProcess))
                importSettings.RecordsToProcess = Int32.Parse(RecordsToProcess);

            var fileInfo = new FileInfo(Filename);
            
            _importService.Import(fileInfo, importSettings);
        }
    }
}