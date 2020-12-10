using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ssdcw.Models.ViewModels
{
    public class EditTicketVM
    {
        public int Id { get; set; }

        public enum Status
        {
            Open,
            In_progress,
            Closed
        }

        [Display(Name = "Status")]
        public Status TicketStatus { get; set; }
    }
}
