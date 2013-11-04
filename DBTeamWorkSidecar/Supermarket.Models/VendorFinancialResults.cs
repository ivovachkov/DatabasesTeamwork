using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermarket.Models
{
    public class VendorFinancialResults
    {
        public string Vendor { get; set; }

        public double Incomes { get; set; }

        public double Expences { get; set; }

        public double Taxes { get; set; }

        public double FinancialResult { get; set; }
    }
}
