using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supermarket.Models;
using Supermarket.OpenAccess.Models;

namespace Supermarket.DbContextSQL
{
    public class SupermarketDB : DbContext
    {
        public SupermarketDB()
            :base("SupermarketDB.SQLServer")
        { }
               
        public DbSet<SaleByDate> SalesByDate { get; set; }
        public DbSet<Supermarket.Models.Supermarket> Supermarkets { get; set; }
        public DbSet<Supermarket.Models.VendorExpence> VendorExpence { get; set; }
        public DbSet<Product> Products { get; set; }       
        public DbSet<Measure> Measures { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
    }
}
