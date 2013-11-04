using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermarket.Models
{
    public class TotalProductSales
    {        
        public ObjectId Id { get; set; }
                
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string VendorName { get; set; }

        public double QuantitySold { get; set; }

        public double TotalIncomes { get; set; }
    }
}
