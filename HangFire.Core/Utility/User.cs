using Steven.HangFire.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Steven.HangFire.Core.Utility
{
    public class User
    {
        public List<CurrentUser>? BasicAuthAuthorizationUser { get; set; }
    }
}
