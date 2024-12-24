using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordo.Models
{
    internal class AppSettings
    {
        public string? ClientId { get; set; }
        public string? TenantId { get; set; }
        public string? ClientSecret { get; set; }
    }
}
