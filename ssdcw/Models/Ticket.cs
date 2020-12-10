using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ssdcw.Models
{
    public class Ticket
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

        [Key]
        public int Id { get; set; }

        [Display(Name ="Created at")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Ticket type")]
        public Type TicketType { get; set; }

        [Display(Name = "Status")]
        public Status TicketStatus { get; set; }

        [Display(Name = "Priority")]
        public Priority TicketPriority { get; set; }

        public string Description { get; set; }

        public Ticket()
        {
            Comments = new List<Comment>();
        }

        public virtual User Author { get; set; }

        [Display(Name = "Assigned to")]
        public virtual User UserAssigned { get; set; }

        public List<Comment> Comments { get; set; }


        public static bool TypeEnumExists(string name)
        {
            if (name.Equals("Development") || name.Equals("Testing") || name.Equals("Production"))
            {
                return true;
            }
            else
                return false;
        }

        public static bool PriorityEnumExists(string name)
        {
            if (name.Equals("Low") || name.Equals("Medium") || name.Equals("High"))
            {
                return true;
            }
            else
                return false;
        }
        public static bool StatusEnumExists(string name)
        {
            if (name.Equals("Open") || name.Equals("In_Progress") || name.Equals("Closed"))
            {
                return true;
            }
            else
                return false;
        }
    }
}
