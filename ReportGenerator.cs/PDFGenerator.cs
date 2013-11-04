using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supermarket.DbContextSQL;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using System.Data;
using Supermarket.Models;
using System.Globalization;

namespace ReportGenerator.cs
{
    public class PDFGenerator
    {
        private const string GeneratingMsg = "Generating PDF Document...";
        private const string PdfPath = "../../../testTablePDF5.pdf";
        private const string TableHeader = "Aggregated Sales Report";

        public void GeneratePDF(SupermarketDB sqlDbContext)
        {
            Console.WriteLine(GeneratingMsg);

            string path = PdfPath;

            Document doc = new Document();
            FileStream fs = File.Create(path);
            PdfWriter.GetInstance(doc, fs);

            doc.Open();

            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;

            var colWidthPercentages = new[] { 70f, 20f, 20f, 70f, 20f };
            table.SetWidths(colWidthPercentages);  

            var cell = new PdfPCell(new Phrase(TableHeader));
            cell.Colspan = 5;
            cell.Padding = 20;
            cell.HorizontalAlignment = 1;

            table.AddCell(cell);

            DateTime currentDate = DateTime.Now;
            decimal sum = -1;
            decimal grandTotal = 0;

            var query = sqlDbContext.Database
                .SqlQuery<AggregatedReportModel>(@"select p.ProductName as Product,
                                                    sd.Quantity as Quantity,
                                                    sd.UnitPrice as UnitPrice,
                                                    s.StoreName as Location,
                                                    m.MeasureName as MeasureName,
                                                    sd.Sum as Sum, 
                                                    sd.Date as Date
                                                   from SaleByDates sd
                                                   join Products p on sd.ProductId = p.ID
                                                   join Measures m on m.ID = p.Measure_ID
                                                   join Supermarkets s on s.Id = sd.SupermarketId
                                                   group by sd.Date, p.ProductName, sd.Quantity, m.MeasureName,
                                                            sd.UnitPrice, s.StoreName, sd.Sum")
                                                    .ToList();

            foreach (var item in query)
            {
                if (item.Date != currentDate)
                {
                    currentDate = item.Date;
                    if (sum != -1)
                    {
                        var totalSumTextCell = new PdfPCell(new Phrase("Total sum for " + item.Date.ToString("d-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-US")) + ":"));
                        totalSumTextCell.HorizontalAlignment = 2;
                        totalSumTextCell.Colspan = 4;
                        table.AddCell(totalSumTextCell);

                        var totalSumCell = new PdfPCell(new Phrase(sum.ToString()));
                        table.AddCell(totalSumCell);

                        sum = -1;
                    }

                    var newDateCell = new PdfPCell(new Phrase("Date: " + item.Date.ToString("d-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-US"))));
                    newDateCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    newDateCell.Colspan = 5;
                    table.AddCell(newDateCell);

                    var product = new PdfPCell(new Phrase("Product"));
                    product.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(product);

                    var quantity = new PdfPCell(new Phrase("Quantity"));
                    quantity.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(quantity);

                    var unitPrice = new PdfPCell(new Phrase("Unit Price"));
                    unitPrice.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(unitPrice);

                    var location = new PdfPCell(new Phrase("Location" ));
                    location.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(location);

                    var sumHeaderCell = new PdfPCell(new Phrase("Sum"));
                    sumHeaderCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(sumHeaderCell);
                    
                }

                table.AddCell(item.Product);
                table.AddCell(item.Quantity + " " + item.Measurename);
                table.AddCell(item.UnitPrice.ToString());
                table.AddCell(item.Location);
                table.AddCell(item.Sum.ToString());

                sum += item.Sum;
                grandTotal += item.Sum;
            }

            var totalSumTextCellEnd = new PdfPCell(new Phrase("Total sum for " + currentDate.ToString("d-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-US")) + ":"));
            totalSumTextCellEnd.HorizontalAlignment = 2;
            totalSumTextCellEnd.Colspan = 4;
            table.AddCell(totalSumTextCellEnd);

            var totalSumCellEnd = new PdfPCell(new Phrase(sum.ToString()));
            table.AddCell(totalSumCellEnd);


            var grandTotalSumTextCellEnd = new PdfPCell(new Phrase("Grand Total:"));
            grandTotalSumTextCellEnd.HorizontalAlignment = 2;
            grandTotalSumTextCellEnd.Colspan = 4;
            table.AddCell(grandTotalSumTextCellEnd);

            var grandTotalCell = new PdfPCell(new Phrase(grandTotal.ToString()));
            table.AddCell(grandTotalCell);
            
            doc.Add(table);
            doc.Close();

        }
    }
}
