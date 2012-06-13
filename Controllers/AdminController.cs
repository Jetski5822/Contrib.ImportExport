using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Contrib.ImportExport.Extensions;
using Contrib.ImportExport.Models;
using Contrib.ImportExport.Services;
using Contrib.ImportExport.ViewModels;
using Orchard;
using Orchard.Blogs.Services;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.UI.Admin;
using Orchard.Utility.Extensions;

namespace Contrib.ImportExport.Controllers {
    [ValidateInput(false), Admin]
    public class AdminController : Controller {
        private readonly IImportService _importService;
        private readonly IBlogService _blogService;

        public AdminController(IOrchardServices services, IImportService importService, IBlogService blogService) {
            Services = services;
            _importService = importService;
            _blogService = blogService;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        public ActionResult Import() {
            if (!Services.Authorizer.Authorize(Permissions.ImportBlog, T("Cannot Import Blog")))
                return new HttpUnauthorizedResult();

            var blogs = _blogService.Get().Select(o => new KeyValuePair<int, string>(o.Id, o.Name)).ToReadOnlyCollection();
            
            return new ShapeResult(this, new ImportAdminViewModel { Settings = new ImportSettings { SlugPattern = @"/([^/]+)\.aspx" }, Blogs = blogs });
        }

        [HttpPost, ActionName("Import")]
        public ActionResult ImportPost(FormCollection formCollection) {
            if (!Services.Authorizer.Authorize(Permissions.ImportBlog, T("Cannot Import Blog")))
                return new HttpUnauthorizedResult();
            
            var viewModel = new ImportAdminViewModel();
            UpdateModel(viewModel, formCollection);

            if (ModelState.IsValid) {
                if (!string.IsNullOrWhiteSpace(viewModel.Settings.UrlItemPath)) {
                    if (viewModel.Settings.UrlItemPath.IsValidUrl())
                        _importService.Import(viewModel.Settings.UrlItemPath, viewModel.Settings);
                    else
                        ModelState.AddModelError("File", T("Invalid Url specified").ToString());
                }
                else {
                    var httpPostedFileBase = Request.Files[0];
                    if (httpPostedFileBase != null && !string.IsNullOrWhiteSpace(httpPostedFileBase.FileName)) {
                        foreach (HttpPostedFileBase file in from string fileName in Request.Files select Request.Files[fileName]) {
                            _importService.Import(file, viewModel.Settings);
                        }
                    }
                    else
                        ModelState.AddModelError("File", T("Select a file to upload").ToString());
                }
            }

            viewModel.Blogs = _blogService.Get().Select(o => new KeyValuePair<int, string>(o.Id, o.Name)).ToReadOnlyCollection();

            return new ShapeResult(this, viewModel);
        }

        //public ActionResult Export(int id) {
        //    if (!Services.Authorizer.Authorize(Permissions.ExportBlog, T("Cannot Export Blog")))
        //        return new HttpUnauthorizedResult();

        //    _exportService.Export(id);

        //    return Redirect("~/Admin/Blogs");
        //}
    }
}