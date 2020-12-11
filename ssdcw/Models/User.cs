using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ssdcw.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Rolename { get; set; }
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        public virtual IEnumerable<Comment> Comments { get; set; }

        [InverseProperty("Author")]
        public virtual IEnumerable<Ticket> TicketsCreated { get; set; }

        [InverseProperty("UserAssigned")]
        public virtual IEnumerable<Ticket> TicketsAssigned { get; set; }


    }
}
