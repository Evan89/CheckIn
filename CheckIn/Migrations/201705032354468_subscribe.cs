namespace CheckIn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subscribe : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserCheckIns", "subscription", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserCheckIns", "subscription");
        }
    }
}
