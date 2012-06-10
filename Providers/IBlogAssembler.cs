using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Contrib.ExternalImportExport.InternalSchema;

namespace Contrib.ExternalImportExport.Providers {
    public interface IBlogAssembler {
        Blog Assemble(Stream stream);
    }
}
