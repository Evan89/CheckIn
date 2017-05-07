namespace CheckIn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class secStringAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserCheckIns", "secString", c => c.String());
            DropColumn("dbo.UserCheckIns", "secNum");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserCheckIns", "secNum", c => c.Int(nullable: false));
            DropColumn("dbo.UserCheckIns", "secString");
        }
    }
}
