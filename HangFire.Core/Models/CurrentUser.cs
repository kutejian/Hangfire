using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Steven.HangFire.Core.Models
{
    public class CurrentUser
    {
        public int Id { get; set; }

        [Display(Name = "用户名")]
        public string? Name { get; set; }

        [Display(Name = "密码")]
        public string? Password { get; set; }
    }
}
