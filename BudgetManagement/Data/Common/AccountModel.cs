using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionAccSystem.Data.Common
{
    public class AccountModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Cell { get; set; } = string.Empty;
        public string WebSiteLink { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
    }
}
