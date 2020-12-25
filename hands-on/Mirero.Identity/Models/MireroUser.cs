using System;
using System.Collections.Generic;

namespace Mirero.Identity.Models
{
    public class MireroUser
    {
        public MireroUser()
        {
            Roles = new List<string>();
        }
        public string KeyOrId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}