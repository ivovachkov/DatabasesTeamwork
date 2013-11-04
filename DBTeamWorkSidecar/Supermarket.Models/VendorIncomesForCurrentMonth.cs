using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermarket.Models
{
    public class VendorIncomesForCurrentMonth
    {

        public string Vendor { get; set; }

        public int VendorId { get; set; }
        
        public string Product { get; set; }

        public double Incomes { get; set; }

        
        public decimal Expenses { get; set; }

        public decimal Taxes { get; set; }


        public double FinancialResult { get; set; }
    }
}
