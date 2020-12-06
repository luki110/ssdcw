using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ssdcw.Models.ViewModels
{
    public class ChangeRoleVM
    {
        [NotMapped]
        public List<string> Rolenames { get; set; }
        public string previousRoleName { get; set; }

        public string newRole { get; set; }
        public User user { get; set; }
        public string UserId { get; set; }
        public ChangeRoleVM()
        {
            Rolenames = new List<string>();            
        }
    }
}
