using System;
using System.Linq;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.IO.Compression;
using Supermarket.DbContextSQL;
using Supermarket.Models;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        ExtractFromExcel();
    }

    static void ExtractFromExcel()
    {
        DirectoryInfo root = new DirectoryInfo("../../../../ZippedDailyReports");

        if (root.GetDirectories().Length == 0)
        {
            string zipPath = "../../../../ZippedDailyReports/Daily-Reports.zip";
            string extractPath = "../../../../ZippedDailyReports/Daily-Reports";
            ZipFile.ExtractToDirectory(zipPath, extractPath);
        }

        DirectoryInfo dir = new DirectoryInfo("../../../../ZippedDailyReports/Daily-Reports");

        var directories = dir.GetDirectories();

        foreach (var directory in directories)
        {
            var date = DateTime.Parse(directory.Name);
            var files = directory.GetFiles();
            foreach (var file in files)
            {
                string fileName = file.FullName;
                DataSet sheet = new DataSet();
                OleDbConnectionStringBuilder conString = new OleDbConnectionStringBuilder();
                conString.Provider = "Microsoft.ACE.OLEDB.12.0";
                conString.DataSource = fileName;
                conString.Add("Extended Properties", "Excel 12.0 Xml;HDR=YES");

                using (var dbCon = new OleDbConnection(conString.ConnectionString))
                {
                    dbCon.Open();
                    string command = "SELECT * FROM [Sales$]";
                    using (var adapter = new OleDbDataAdapter(command, dbCon))
                    {
                        adapter.Fill(sheet);
                    }
                }

                DataTable table = sheet.Tables[0];
                string supermarketName = GetSuperMarketName(table);
                AddSupermarket(supermarketName);

                int superMarketId = GetSupermarketId(supermarketName);
                decimal sum = GetSum(table);
                List<decimal[]> reportData = GetReportData(table);


                foreach (var report in reportData)
                {
                    var productId = (int)report[0];
                    var quantity = (int)report[1];
                    var unitPrice = report[2];
                    var reportSum = report[3];

                    var salesReport = new SaleByDate
                    {
                        Date = date,
                        Sum = reportSum,
                        SupermarketId = superMarketId,
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = unitPrice
                    };

                    using (var db = new SupermarketDB())
                    {
                        db.SalesByDate.Add(salesReport);
                        db.SaveChanges();
                    }
                }
            }
        }
    }

    static decimal GetSum(DataTable table)
    {
        return decimal.Parse(table.Rows[table.Rows.Count - 1][table.Columns.Count - 1].ToString());
    }

    static List<decimal[]> GetReportData(DataTable table)
    {
        List<decimal[]> allData = new List<decimal[]>();

        for (int row = 2; row < table.Rows.Count - 1; row++)
        {
            DataRow dataRow = table.Rows[row];
            var productData = new decimal[4];
            decimal result = 0;
            for (int col = 0; col < 4; col++)
            {
                DataColumn dataCol = table.Columns[col];

                decimal.TryParse(dataRow[dataCol].ToString(), out result);
                productData[col] = result;
            }
            if (result != 0)
            {
                allData.Add(productData);
            }
        }

        return allData;
    }

    static int GetSupermarketId(string name)
    {
        using (var db = new SupermarketDB())
        {
            return db.Supermarkets.First(x => x.StoreName == name).Id;
        }
    }

    static string GetSuperMarketName(DataTable table)
    {
        DataRow row = table.Rows[0];
        DataColumn col = table.Columns[0];
        string line = row[col].ToString();
        int spaceIndex = line.IndexOf(' ');
        string name = line.Substring(spaceIndex + 1);

        return name;
    }

    static void AddSupermarket(string name)
    {
        using (var db = new SupermarketDB())
        {
            var count = db.Supermarkets.Where(x => x.StoreName == name).Count();

            if (count == 0)
            {
                var supermarket = new Supermarket.Models.Supermarket { StoreName = name };
                db.Supermarkets.Add(supermarket);
                db.SaveChanges();
            }
        }
    }
}