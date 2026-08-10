namespace RestaurantManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRecipientDetailsToUserAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserAddresses", "RecipientName", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.UserAddresses", "Phone", c => c.String(nullable: false, maxLength: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserAddresses", "Phone");
            DropColumn("dbo.UserAddresses", "RecipientName");
        }
    }
}
