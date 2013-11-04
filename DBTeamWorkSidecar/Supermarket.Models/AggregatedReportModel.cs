using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermarket.Models
{
    public class AggregatedReportModel
    {
        public DateTime Date { get; set; }
        public string Product { get; set; }
        public float Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Location { get; set; }
        public decimal Sum { get; set; }
        public string Measurename { get; set; }
    }
}
