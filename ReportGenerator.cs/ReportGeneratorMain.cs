using Supermarket.DbContextSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenerator.cs
{
    class ReportGeneratorMain
    {
        static void Main(string[] args)
        {
            PDFGenerator pdf = new PDFGenerator();
            XMLGenerator xml = new XMLGenerator();

            SupermarketDB dbContext = new SupermarketDB();

            using (dbContext)
            {
                pdf.GeneratePDF(dbContext);

                xml.GenerateXML(dbContext);
            }
        }
    }
}
