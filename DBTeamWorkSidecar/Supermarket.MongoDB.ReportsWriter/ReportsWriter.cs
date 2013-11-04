using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using Supermarket.Models;
using Supermarket.DbContextSQL;
using Supermarket.OpenAccess;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Options;
using System.Text.RegularExpressions;
using System.IO;


namespace Supermarket.MongoDBTest
{
    //  mongodb://<dbuser>:<dbpassword>@ds037358.mongolab.com:37358/teamwork-sidecar
    
    class Program
    {
        const string CollectionReportsTest = "ReportsTest";
        const string Local = "mongodb://localhost";
        const string ConnectionString =@"mongodb://parlakov:parlakov@ds037358.mongolab.com:37358";
        const string Database = "teamwork-sidecar";

        const string ProductId = "product-id";
        const string ProductName = "product-name";
        const string VendorName = "vendor-name";
        const string TotalQuantity = "total-quantity-sold";
        const string TotalIncome = "total-incomes";

        const string FilesPath = "..\\..\\..\\..\\Product-Reports\\";
        const string FileExt = ".json";

        static void Main(string[] args)
        {
            using (var supermarketDB = new SupermarketDB())
            {
                Console.WriteLine("Getting data from SQL server....");
                var productReports = GetTotalProductSales(supermarketDB);

                var client = new MongoClient(Local);

                Console.WriteLine("Saving product reports to MongoDb (Local)....");
                AllSalesReportsSqlToMongo(supermarketDB, productReports, client);

                //TestSaved(client);                             

                Console.WriteLine("Generating jsonFiles - output to {0}", FilesPath);
                GenerateJsonReportFilesFiles(client);
            }
        }

        private static void GenerateJsonReportFilesFiles(MongoClient client)
        {
            var reportsFromMongo = GetReportsFromMongo(client);
            var matchMongoIdRegEx = new Regex("\"_id.+?,");
            foreach (var report in reportsFromMongo)
            {
                using (var writer = new StreamWriter(FilesPath + report[ProductId] + FileExt))
                {
                    var json = report.ToJson<BsonDocument>();
                    //var json = report.ToString();
                    json = matchMongoIdRegEx.Replace(json, "");
                    json = json.Replace(",", ",\n").Replace("{ ", "{\n").Replace("}", "\n}");
                    writer.Write(json);
                }
            }
        }

        private static MongoCursor<BsonDocument> GetReportsFromMongo(MongoClient client)
        {
            var db = client.GetServer().GetDatabase(Database);
            var collection = db.GetCollection(CollectionReportsTest).FindAll();

            return collection;
        }

        private static void TestSaved(MongoClient client)
        {
            var db = client.GetServer().GetDatabase(Database);
            var collection = db.GetCollection(CollectionReportsTest).FindAll();
            
            foreach (var item in collection)
            {
                Console.WriteLine("{0} -> {1}", item[ProductName], item[TotalQuantity]);
            }
        }

        private static object GetJsonCollection(MongoCursor<BsonDocument> collection)
        {
            var jsonCollection = new DoubleJsonToken("0,00", 2.54545d);
            return jsonCollection;
        }

        private static void AllSalesReportsSqlToMongo(
            SupermarketDB supermarketDB, List<TotalProductSales> productReports, MongoClient client)
        {
            var db = client.GetServer().GetDatabase(Database);
            if (!db.CollectionExists(CollectionReportsTest))
            {
                db.CreateCollection(CollectionReportsTest);
            }
            db.DropCollection(CollectionReportsTest);
            db.CreateCollection(CollectionReportsTest);

            RegisterCustomSerializer();

            var collection = db.GetCollection<TotalProductSales>(CollectionReportsTest);
            collection.InsertBatch(productReports);
        }

        private static void RegisterCustomSerializer()
        {
            var map = BsonClassMap.RegisterClassMap<TotalProductSales>(cm =>
            {
                cm.AutoMap();
                cm.GetMemberMap(t => t.ProductId).SetElementName(ProductId).SetOrder(1);
                cm.GetMemberMap(t => t.ProductName).SetElementName(ProductName).SetOrder(2);
                cm.GetMemberMap(t => t.VendorName).SetElementName(VendorName).SetOrder(3);
                cm.GetMemberMap(t => t.QuantitySold).SetElementName(TotalQuantity).SetOrder(4);
                cm.GetMemberMap(t => t.TotalIncomes).SetElementName(TotalIncome).SetOrder(5);
            });                        
        }

        private static List<TotalProductSales> GetTotalProductSales(SupermarketDB supermarketDB)
        {
            var productReports = supermarketDB.Database.SqlQuery<TotalProductSales>(@"
                    SELECT  SUM(s.UnitPrice * s.Quantity) as TotalIncomes,
		                    SUM(s.Quantity) as [QuantitySold],		
		                    s.ProductId ,
		                    MAX(p.ProductName) as ProductName,
		                    MAX(v.VendorName) as VendorName
                    FROM [Supermarket].[dbo].[SaleByDates] s
	                    JOIN Products p ON s.ProductId = p.ID
	                    JOIN Vendors v ON p.Vendor_ID = v.ID
                    GROUP BY s.ProductId
                    ORDER BY s.ProductId").ToList();
            return productReports;
        }
    }
}
