using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Contrib.ImportExport.InternalSchema;
using Orchard;

namespace Contrib.ImportExport.Providers {
    public interface IBlogAssembler : IDependency {
        string Name { get; }
        bool IsFeed { get; }
        Blog Assemble(Stream stream);
    }
}
