namespace RestaurantManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MenuItems",
                c => new
                    {
                        MenuItemId = c.Long(nullable: false, identity: true),
                        RestaurantId = c.Long(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 250),
                        Price = c.Decimal(nullable: false, precision: 10, scale: 4),
                        Quantity = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MenuItemId)
                .ForeignKey("dbo.Restaurants", t => t.RestaurantId)
                .Index(t => t.RestaurantId);
            
            CreateTable(
                "dbo.Restaurants",
                c => new
                    {
                        RestaurantId = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Address = c.String(nullable: false, maxLength: 500),
                        OwnerId = c.Long(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RestaurantId)
                .ForeignKey("dbo.Users", t => t.OwnerId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        RestaurantId = c.Long(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        Status = c.Byte(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 10, scale: 4),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Restaurants", t => t.RestaurantId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RestaurantId);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        OrderItemId = c.Long(nullable: false, identity: true),
                        OrderId = c.Long(nullable: false),
                        MenuItemId = c.Long(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 10, scale: 4),
                    })
                .PrimaryKey(t => t.OrderItemId)
                .ForeignKey("dbo.MenuItems", t => t.MenuItemId)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .Index(t => t.OrderId)
                .Index(t => t.MenuItemId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 100),
                        PasswordHash = c.String(nullable: false, maxLength: 256),
                        Phone = c.String(maxLength: 10),
                        RoleId = c.Int(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 10, scale: 4),
                        UpdatedAt = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .Index(t => t.Email, unique: true, name: "IX_User_Email")
                .Index(t => t.Phone, unique: true, name: "IX_User_Phone")
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        RefreshTokenId = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        TokenHash = c.String(nullable: false, maxLength: 256),
                        ExpiresAt = c.DateTime(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        RevokedAt = c.DateTime(),
                        IsRevoked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RefreshTokenId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.TokenHash, unique: true, name: "IX_RefreshToken_TokenHash");
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        Name = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.RoleId)
                .Index(t => t.Name, unique: true, name: "IX_Role_Name");
            
            CreateTable(
                "dbo.UserAddresses",
                c => new
                    {
                        UserAddressId = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        AddressLine1 = c.String(nullable: false, maxLength: 255),
                        AddressLine2 = c.String(maxLength: 255),
                        City = c.String(nullable: false, maxLength: 100),
                        State = c.String(nullable: false, maxLength: 100),
                        PostalCode = c.String(nullable: false, maxLength: 20),
                        Country = c.String(nullable: false, maxLength: 100),
                        Landmark = c.String(maxLength: 255),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserAddressId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.OrderAddresses",
                c => new
                    {
                        OrderAddressId = c.Long(nullable: false, identity: true),
                        OrderId = c.Long(nullable: false),
                        RecipientName = c.String(nullable: false, maxLength: 100),
                        Phone = c.String(nullable: false, maxLength: 10),
                        AddressLine1 = c.String(nullable: false, maxLength: 255),
                        AddressLine2 = c.String(maxLength: 255),
                        City = c.String(nullable: false, maxLength: 100),
                        State = c.String(nullable: false, maxLength: 100),
                        PostalCode = c.String(nullable: false, maxLength: 20),
                        Country = c.String(nullable: false, maxLength: 100),
                        Landmark = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.OrderAddressId)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .Index(t => t.OrderId, unique: true, name: "IX_OrderAddress_OrderId");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderAddresses", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.MenuItems", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.Restaurants", "OwnerId", "dbo.Users");
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserAddresses", "UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.RefreshTokens", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "RestaurantId", "dbo.Restaurants");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderItems", "MenuItemId", "dbo.MenuItems");
            DropIndex("dbo.OrderAddresses", "IX_OrderAddress_OrderId");
            DropIndex("dbo.UserAddresses", new[] { "UserId" });
            DropIndex("dbo.Roles", "IX_Role_Name");
            DropIndex("dbo.RefreshTokens", "IX_RefreshToken_TokenHash");
            DropIndex("dbo.RefreshTokens", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "RoleId" });
            DropIndex("dbo.Users", "IX_User_Phone");
            DropIndex("dbo.Users", "IX_User_Email");
            DropIndex("dbo.OrderItems", new[] { "MenuItemId" });
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.Orders", new[] { "RestaurantId" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.Restaurants", new[] { "OwnerId" });
            DropIndex("dbo.MenuItems", new[] { "RestaurantId" });
            DropTable("dbo.OrderAddresses");
            DropTable("dbo.UserAddresses");
            DropTable("dbo.Roles");
            DropTable("dbo.RefreshTokens");
            DropTable("dbo.Users");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Orders");
            DropTable("dbo.Restaurants");
            DropTable("dbo.MenuItems");
        }
    }
}
