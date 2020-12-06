using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ssdcw.Models
{
   
        public class Comment
        {
            [Key]
            public int Id { get; set; }
            public string Content { get; set; }
            public DateTime? DatePosted { get; set; }

            //Navigational properties
            [InverseProperty("User")]
            public string UserId { get; set; }
            public virtual User User { get; set; }

            [InverseProperty("Ticket")]
            public int TicketId { get; set; }
            public virtual Ticket Ticket { get; set; }
        }

}
