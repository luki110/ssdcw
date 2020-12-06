using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ssdcw.Models.ViewModels
{
    public class TablePartialVM
    {
        public int Id { get; set; }
        public string Controller { get; set; }

        public TablePartialVM()
        {

        }

        public TablePartialVM(int id, string controller)
        {
            Id = id;
            Controller = controller;
        }
    }
}
