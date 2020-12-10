using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ssdcw.Models.ViewModels
{
    public class TicketWithCommentsVM
    {
        public Ticket ticket { get; set; }

        public User author { get; set; }

        public User userAssigned { get; set; }

        public DateTime Datecreated { get; set; }
        public List<Comment> comments { get; set; }

        public TicketWithCommentsVM()
        {
            comments = new List<Comment>();
            
        }

    }
}
