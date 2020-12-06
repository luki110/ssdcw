using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ssdcw.Models
{
    public class User : IdentityUser
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Rolename { get; set; }
        public virtual IEnumerable<Comment> Comments { get; set; }

        [InverseProperty("Author")]
        public virtual IEnumerable<Ticket> TicketsCreated { get; set; }

        [InverseProperty("UserAssigned")]
        public virtual IEnumerable<Ticket> TicketsAssigned { get; set; }


    }
}
