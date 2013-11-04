using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermarket.Models
{
    public class SaleByDate
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int SupermarketId { get; set; }

        public int ProductId { get; set; }

        public float Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Sum { get; set; }
    }
}
