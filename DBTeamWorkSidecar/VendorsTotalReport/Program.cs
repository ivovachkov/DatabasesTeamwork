using System;
using System.Collections.Generic;
using System.Data.SQLite;
using VendorsTotalReport;
using Supermarket.DbContextSQL;
using Supermarket.Models;
using Supermarket.OpenAccess;
using Telerik.OpenAccess;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using System.Data;
using System.Linq;
using System.Globalization;
using System.Data.OleDb;

class Program
{
    const string MongoUrl = "mongodb://localhost/";
    const string MongoDatabase = "teamwork-sidecar";
    const string MongoCollection = "expences";

    const string FullFilePath = "..\\..\\..\\Vendor-Results\\Products-Total-Report.xlsx";

    static void Main()
    {
        var expences = GetExpencesFromMongoDb();

        var productTaxes = GetVendorTaxesFromSQLite();

        var incomesCurrentMonth = GetVendorIncomesFromSQLServer();

        CalculateFinancialResults(productTaxes, incomesCurrentMonth, expences);

        var vendorResults = ConvertToVendorFinancialResults(incomesCurrentMonth);

        InsertFinancialResultsToSQLite(vendorResults);

        InsertIntoExcel();
    }

    private static void InsertFinancialResultsToSQLite(Dictionary<string, VendorFinancialResults> vendorResults)
    {
        var connection = new SQLiteConnection(Settings.Default.DBConnectionString);

        connection.Open();

        using (connection)
        {﻿
            foreach (var result in vendorResults)
            {
                var command = new SQLiteCommand("INSERT INTO ProductsTotalReport(VendorName, Incomes, Expences, Taxes, FinancialResult)  " +
                    "VALUES (@vendor, @incomes, @expences, @taxes, @financialResults)", connection);
                command.Parameters.AddWithValue("@vendor", result.Value.Vendor);
                command.Parameters.AddWithValue("@incomes", result.Value.Incomes);
                command.Parameters.AddWithValue("@expences", result.Value.Expences);
                command.Parameters.AddWithValue("@taxes", result.Value.Taxes);
                command.Parameters.AddWithValue("@financialResults", result.Value.FinancialResult);

                command.ExecuteNonQuery();
            }
        }
    }

    private static void InsertIntoExcel()
    {
        OleDbConnectionStringBuilder conString = new OleDbConnectionStringBuilder();
        conString.Provider = "Microsoft.ACE.OLEDB.12.0";
        conString.DataSource = FullFilePath;
        conString.Add("Extended Properties", "Excel 12.0 Xml;HDR=YES");

        using (var dbCon = new OleDbConnection(conString.ConnectionString))
        {
            dbCon.Open();

            var command = new OleDbCommand("INSERT INTO [Sheet1$] VALUES (@vendor, @incomes, @expences, @taxes, @financialResults)");
            command.Parameters.AddWithValue("@vendor", "Vendor");
            command.Parameters.AddWithValue("@incomes", "Incomes");
            command.Parameters.AddWithValue("@expences", "Expences");
            command.Parameters.AddWithValue("@taxes", "Taxes");
            command.Parameters.AddWithValue("@finanvialResults", "Financial Results");


            //string command = "SELECT * FROM [Sales$]";
            //using (var adapter = new OleDbDataAdapter(command, dbCon))
            //{
            //    adapter.Fill(sheet);
            //}
        }
    }

    private static Dictionary<string, VendorFinancialResults>
        ConvertToVendorFinancialResults(List<VendorIncomesForCurrentMonth> incomesCurrentMonth)
    {
        var vendorResults = new Dictionary<string, VendorFinancialResults>();

        foreach (var item in incomesCurrentMonth)
        {
            var vendor = item.Vendor;

            if (!vendorResults.ContainsKey(vendor))
            {
                vendorResults.Add(vendor, new VendorFinancialResults
                {
                    Vendor = item.Vendor,
                    Taxes = (double)item.Taxes,
                    Incomes = item.Incomes,
                    Expences = (double)item.Expenses,
                    FinancialResult = item.FinancialResult
                });
            }
            else
            {
                vendorResults[vendor].FinancialResult += item.FinancialResult;
            }
        }
        return vendorResults;
    }

    static MongoCollection InitializeMongoClient()
    {
        MongoClient mongoClient = new MongoClient(MongoUrl);
        MongoServer mongoServer = mongoClient.GetServer();
        MongoDatabase supermarket = mongoServer.GetDatabase(MongoDatabase);

        return supermarket.GetCollection(MongoCollection);
    }

    #region GetDataFromservers methods
    private static List<VendorExpence> GetExpencesFromMongoDb()
    {
        var currMonth = DateTime.Now.ToString("MMM", CultureInfo.InvariantCulture);

        MongoCollection expencesCollection = InitializeMongoClient();
        var expences = expencesCollection.AsQueryable<VendorExpence>()
            .Where(x => x.Month.StartsWith(currMonth)).ToList();

        return expences;
    }

    private static void CalculateFinancialResults(
        Dictionary<string, decimal> productTaxes,
        List<VendorIncomesForCurrentMonth> incomesCurrentMonth,
        List<VendorExpence> expences)
    {
        foreach (var item in incomesCurrentMonth)
        {
            var tax = productTaxes[item.Product] / 100;
            item.Taxes = (decimal)item.Incomes * tax;

            var expence = expences.Find(ex => ex.VendorId == item.VendorId).Ammount;
            item.Expenses = expence;

            item.FinancialResult = item.Incomes - (double)(item.Taxes + item.Expenses);
        }
    }

    private static Dictionary<string, decimal> GetVendorTaxesFromSQLite()
    {
        var vendorsTaxes = new Dictionary<string, decimal>();
        var connection = new SQLiteConnection(Settings.Default.DBConnectionString);
        connection.Open();
        using (connection)
        {
            var command = new SQLiteCommand("SELECT ProductName, Tax FROM VendorTaxes", connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var productName = reader["ProductName"].ToString();
                var tax = decimal.Parse(reader["Tax"].ToString());
                if (!vendorsTaxes.ContainsKey(productName))
                {
                    vendorsTaxes.Add(productName, tax);
                }
            }
        }

        return vendorsTaxes;
    }

    static List<VendorIncomesForCurrentMonth> GetVendorIncomesFromSQLServer()
    {
        using (var supermarketDb = new SupermarketDB())
        {
            var currentMonthReport = supermarketDb.Database
                .SqlQuery<VendorIncomesForCurrentMonth>(@"
                    SELECT  MAX(v.VendorName) as Vendor,
	                    SUM(s.UnitPrice * s.Quantity) as Incomes,
	                    max(p.ProductName) as Product,
	                    max(v.ID) as VendorId
                    FROM [Supermarket].[dbo].[SaleByDates] s
	                    JOIN Products p ON s.ProductId = p.ID
	                    JOIN Vendors v ON p.Vendor_ID = v.ID
                    WHERE YEAR(s.[Date]) = YEAR(GETDATE()) AND MONTH(s.[Date]) = Month(GETDATE())
                    GROUP BY  p.ProductName").ToList();

            return currentMonthReport;
        }
    }
    #endregion
}