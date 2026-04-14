namespace CarSalesAndInventoryManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyMyTable5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Payments", "OrderID", "dbo.Orders");
            DropIndex("dbo.Payments", new[] { "OrderID" });
            DropTable("dbo.Payments");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Method = c.String(nullable: false, maxLength: 50),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentID);
            
            CreateIndex("dbo.Payments", "OrderID");
            AddForeignKey("dbo.Payments", "OrderID", "dbo.Orders", "OrderID", cascadeDelete: true);
        }
    }
}
