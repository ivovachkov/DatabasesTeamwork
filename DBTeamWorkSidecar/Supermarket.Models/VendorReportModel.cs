using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermarket.Models
{
    public class VendorReportModel
    {
        public string Vendor { get; set; }
        public DateTime Date { get; set; }
        public decimal Sum { get; set; }
    }
}
