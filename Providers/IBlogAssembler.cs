using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Contrib.ImportExport.InternalSchema;

namespace Contrib.ImportExport.Providers {
    public interface IBlogAssembler {
        string Name { get; }
        Blog Assemble(Stream stream);
    }
}
