using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ssdcw.Models.ViewModels
{
    public class CreateTicketVM
    {
        public enum Priority
        {
            Low,
            Medium,
            High
        }

        public enum Status
        {
            Open,
            In_progress,
            Closed

        }

        public enum Type
        {
            Development,
            Testing,
            Production
        }
        public DateTime DateCreated { get; set; }

        [Required]
        [Display(Name = "Ticket type")]
        public Type TicketType { get; set; }

        [Display(Name = "Status")]
        public Status TicketStatus { get; set; }

        [Required]
        [Display(Name = "Priority")]
        public Priority TicketPriority { get; set; }

        [Required]
        public string Description { get; set; }

        public List<User> users { get; set; }

        public CreateTicketVM()
        {
            users = new List<User>();
        }
        [Required]
        public string userAssigned { get; set; }
        public string Author { get; set; }

    }
}
