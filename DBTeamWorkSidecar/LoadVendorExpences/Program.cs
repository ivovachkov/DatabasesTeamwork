using Supermarket.DbContextSQL;
using Supermarket.Models;
using System.Linq;
using System.Xml;
using MongoDB.Driver;
using Supermarket.OpenAccess.Models;

class Program
{
    static void Main()
    {
        MongoClient mongoClient = new MongoClient("mongodb://localhost/");
        MongoServer mongoServer = mongoClient.GetServer();
        MongoDatabase supermarket = mongoServer.GetDatabase("teamwork-sidecar");
        MongoCollection expences = supermarket.GetCollection("expences");

        using (var db = new SupermarketDB())
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("../../../../fileVendors-Expenses.xml");
            string salePath = "/sales/sale";
            XmlNodeList sales = xmlDoc.SelectNodes(salePath);
            int count = 1;

            foreach (XmlElement item in sales)
            {
                string name = item.GetAttribute("vendor");
                Vendor vendor = db.Vendors.First(x => x.VendorName == name);
                if (vendor != null)
                {
                    var expencesss = item.SelectNodes("expenses");
                    foreach (XmlElement expence in expencesss)
                    {
                        string month = expence.GetAttribute("month");
                        decimal amount = decimal.Parse(expence.InnerText);
                        var vendorExpence = new VendorExpence();
                        vendorExpence.VendorId = vendor.ID;
                        vendorExpence.Month = month;
                        vendorExpence.Ammount = amount;
                        db.VendorExpence.Add(vendorExpence);

                        // used for MongoDB
                        vendorExpence.Id = count++;
                        AddExpenceToMongoDB(expences, vendorExpence);
                    }
                }
            }

            db.SaveChanges();
        }
    }

    static void AddExpenceToMongoDB(MongoCollection expences, VendorExpence vendorExpence)
    {
        expences.Insert(vendorExpence);
    }
}