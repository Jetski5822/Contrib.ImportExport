using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Contrib.ImportExport {
    public class Permissions : IPermissionProvider {
        public static readonly Permission ImportBlog = new Permission { Description = "Import blog", Name = "ImportBlog" };
        public static readonly Permission ExportBlog = new Permission { Description = "Export blog", Name = "ExportBlog" };
        
        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new Permission[] {
                ImportBlog,
                ExportBlog,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ImportBlog, ExportBlog}
                },
            };
        }

    }
}