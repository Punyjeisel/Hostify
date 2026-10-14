using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hostify.Infrastructure.configuration
{
    public class EmailConfiguration
    {
        public string FromEmail { get; set; } = string.Empty;
        public string AppPassword { get; set; } = string.Empty;
    }
}
