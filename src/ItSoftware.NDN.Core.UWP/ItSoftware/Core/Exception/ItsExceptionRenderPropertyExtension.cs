using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItSoftware.Core.Exception
{
    public class ItsExceptionRenderPropertyExtension
    {
        public string Name { get; set; } = string.Empty;
        public bool IsEnumerable { get; set; } = false;
        public ItsExceptionRenderPropertyExtension[] Properties { get; set; } = null;
    }
}
