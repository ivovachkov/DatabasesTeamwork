//namespace Supermarket.DbContextSQL.Migrations
//{
//    using System;
//    using System.Data.Entity.Migrations;
    
//    public partial class InitialCreate : DbMigration
//    {
//        public override void Up()
//        {
//            CreateTable(
//                "dbo.SaleByDates",
//                c => new
//                    {
//                        Id = c.Int(nullable: false, identity: true),
//                        Date = c.DateTime(nullable: false),
//                        SupermarketId = c.Int(nullable: false),
//                        Quantity = c.Single(nullable: false),
//                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
//                        Sum = c.Decimal(nullable: false, precision: 18, scale: 2),
//                    })
//                .PrimaryKey(t => t.Id)
//                .ForeignKey("dbo.Supermarkets", t => t.SupermarketId, cascadeDelete: true)
//                .Index(t => t.SupermarketId);
            
//            CreateTable(
//                "dbo.Supermarkets",
//                c => new
//                    {
//                        Id = c.Int(nullable: false, identity: true),
//                        StoreName = c.String(),
//                    })
//                .PrimaryKey(t => t.Id);
            
//            CreateTable(
//                "dbo.Products",
//                c => new
//                    {
//                        ID = c.Int(nullable: false, identity: true),
//                        ProductName = c.String(),
//                        BasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
//                        Measures_ID = c.Int(nullable: false),
//                        Vendors_ID = c.Int(nullable: false),
//                        Measure_ID = c.Int(),
//                        Vendor_ID = c.Int(),
//                    })
//                .PrimaryKey(t => t.ID)
//                .ForeignKey("dbo.Measures", t => t.Measure_ID)
//                .ForeignKey("dbo.Vendors", t => t.Vendor_ID)
//                .Index(t => t.Measure_ID)
//                .Index(t => t.Vendor_ID);
            
//            CreateTable(
//                "dbo.Measures",
//                c => new
//                    {
//                        ID = c.Int(nullable: false, identity: true),
//                        MeasureName = c.String(),
//                    })
//                .PrimaryKey(t => t.ID);
            
//            CreateTable(
//                "dbo.Vendors",
//                c => new
//                    {
//                        ID = c.Int(nullable: false, identity: true),
//                        VendorName = c.String(),
//                    })
//                .PrimaryKey(t => t.ID);
            
//        }
        
//        public override void Down()
//        {
//            DropIndex("dbo.Products", new[] { "Vendor_ID" });
//            DropIndex("dbo.Products", new[] { "Measure_ID" });
//            DropIndex("dbo.SaleByDates", new[] { "SupermarketId" });
//            DropForeignKey("dbo.Products", "Vendor_ID", "dbo.Vendors");
//            DropForeignKey("dbo.Products", "Measure_ID", "dbo.Measures");
//            DropForeignKey("dbo.SaleByDates", "SupermarketId", "dbo.Supermarkets");
//            DropTable("dbo.Vendors");
//            DropTable("dbo.Measures");
//            DropTable("dbo.Products");
//            DropTable("dbo.Supermarkets");
//            DropTable("dbo.SaleByDates");
//        }
//    }
//}
