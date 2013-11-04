using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supermarket.OpenAccess.Models;
using Telerik.OpenAccess;
using Supermarket.DbContextSQL;


namespace Supermarket.TransferData
{
    public class InitialTransferer
    {
        static void Main()
        {
            try
            {
                using (var mySqlDbContext = new SupermarketDbMySQL())
                {
                    //Console.WriteLine("Loading products...");
                    //ProductsTransfer(mySqlDbContext);

                    //Console.WriteLine("Loading Remaining Measures...");
                    //TransferRemainingMeasures(mySqlDbContext);

                    //Console.WriteLine("Loading Remaining Vendors...");
                    //TransferRemainingVendors(mySqlDbContext);

                    //TestUploaded();                    
                }
            }
            catch (Exception ex)
            {
                var message = new StringBuilder();
                message.Append(ex.Message);
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    message.AppendLine(ex.Message);
                }

                Console.WriteLine(message);        
            }
        }

        private static void TestUploaded()
        {
            using (var dbContextSuper = new SupermarketDB())
            {
                var products = dbContextSuper.Products.Include("Measure").Include("Vendor");
                foreach (var product in products)
                {
                    Console.WriteLine(product.Vendor.VendorName + " " + product.Vendors_ID);
                }
            }
        }

        private static void ProductsTransfer(SupermarketDbMySQL mySqlDbContext)
        {            
            using (var sqlDbContext = new SupermarketDB())
            {
                var uploadedProductsIds = sqlDbContext.Products.Select(p => p.ID).ToList();
                var uploadedVendorsIds = sqlDbContext.Vendors.Select(v => v.ID).ToList();
                var uploadedMeasuresIds = sqlDbContext.Measures.Select(m => m.ID).ToList();

                var newProducts = mySqlDbContext.Products
                    .Include(x => x.Vendor)
                    .Include(x => x.Measure)
                    .Where(p => !uploadedProductsIds.Contains(p.ID)).ToList();

                UploadNewProducts(sqlDbContext, uploadedVendorsIds, uploadedMeasuresIds, newProducts);
                               
                sqlDbContext.SaveChanges();
            }
        }

        private static void UploadNewProducts(
            SupermarketDB sqlDbContext, List<int> uploadedVendorsIds,
            List<int> uploadedMeasuresIds, List<Product> products)
        {
            foreach (var product in products)
            {                
                if (uploadedVendorsIds.Contains(product.Vendor.ID))
                {
                    sqlDbContext.Vendors.Attach(product.Vendor);
                }
                else
                {
                    var newVendor = new Vendor { VendorName = product.Vendor.VendorName, ID = product.Vendor.ID };
                    sqlDbContext.Vendors.Add(newVendor);
                    product.Vendor = newVendor;
                }

                if (uploadedMeasuresIds.Contains(product.Measure.ID))
                {
                    sqlDbContext.Measures.Attach(product.Measure);
                }
                else
                {
                    var newMeasure = new Measure { MeasureName = product.Measure.MeasureName, ID = product.Measure.ID };
                    sqlDbContext.Measures.Add(newMeasure);
                    product.Measure = newMeasure;
                }

                sqlDbContext.Products.Add(product);
            }
        }

        private static void TransferRemainingMeasures(SupermarketDbMySQL mySqlDbContext)
        {           
            using (var sqlDbContext = new SupermarketDB())
            {
                var uploadedMeasuresIds = sqlDbContext.Measures.Select(m => m.ID).ToList();

                var measures = mySqlDbContext.Measures.Where(m => !uploadedMeasuresIds.Contains(m.ID)).ToList();

                foreach (var measure in measures)
                {
                    sqlDbContext.Measures.Add(measure);                                                          
                }                

                sqlDbContext.SaveChanges();                
            }
        }

        private static void TransferRemainingVendors(SupermarketDbMySQL mySqlDbContext)
        {           
            using (var sqlDbContext = new SupermarketDB())
            {
                var uploadedVendorsIds = sqlDbContext.Vendors.Select(v => v.ID).ToList();

                var vendors = mySqlDbContext.Vendors.Where(v => !uploadedVendorsIds.Contains(v.ID)).ToList();           
               
                foreach (var vendor in vendors)
                {
                    sqlDbContext.Vendors.Add(vendor);                     
                }

                sqlDbContext.SaveChanges();
            }
        }
    }
}
