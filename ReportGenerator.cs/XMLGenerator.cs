using Supermarket.DbContextSQL;
using Supermarket.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ReportGenerator.cs
{
    public class XMLGenerator
    {
        public void GenerateXML(SupermarketDB sqlDbContext)
        {
            Console.WriteLine("Generating XML....");

            string fileName = "../../../sales.xml";
            Encoding encoding = Encoding.GetEncoding("utf-8");


            using (XmlTextWriter writer = new XmlTextWriter(fileName, encoding))
            {
                writer.Formatting = Formatting.Indented;
                writer.IndentChar = '\t';
                writer.Indentation = 1;

                writer.WriteStartDocument();
                writer.WriteStartElement("sales");

                var query = sqlDbContext.Database.SqlQuery<VendorReportModel>
                                 (@"  select v.VendorName as Vendor, sd.Date as Date, sd.Sum as Sum
                                  from SaleByDates sd
                                  join Products p on sd.ProductId = p.ID
                                  join Vendors v on v.ID = p.Vendor_ID  
                                  group by v.VendorName, sd.Date, sd.Sum").ToList();

                DateTime currentDate = DateTime.Now; //query[0].Date;
                string currentVendor = string.Empty;
                decimal currentSum = 0;

                bool isOpened = false;

                foreach (var item in query)
                {
                    if (item.Vendor != currentVendor)
                    {
                        if (isOpened)
                        {
                            writer.WriteEndElement();
                        }
                        writer.WriteStartElement("sale");
                        isOpened = true;
                        writer.WriteAttributeString("vendor", item.Vendor);
                        currentVendor = item.Vendor;
                    }

                    currentSum += item.Sum;

                    if (item.Date != currentDate)
                    {
                        currentDate = item.Date;
                        WriteSale(writer, item.Date, currentSum);
                        currentSum = 0;
                    }

                    
                }

                if (isOpened)
                {
                    writer.WriteEndElement();
                }

                writer.WriteEndDocument();
            }
        }

        private void WriteSale(XmlWriter writer, DateTime date, decimal totalSum)
        {
            writer.WriteStartElement("summary");
            writer.WriteAttributeString("date", date.ToString("d-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-US")));
            writer.WriteAttributeString("totalSum", totalSum.ToString());

            writer.WriteEndElement();
        }
    }
}
