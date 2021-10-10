using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasestudyAPI.Helpers
{
    public class OrderDetailsHelper
    {
        public int OrderId { get; set; }
        public string ProductId { get; set; }
        public int Qty { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public string DateCreated { get; set; }
        public int QtyOrdered { get; set; }
        public int QtySold { get; set; }
        public int QtyB { get; set; }
        public decimal Extend { get; set; }
        public decimal SubTotal { get; set; }
    }
}

