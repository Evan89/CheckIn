namespace CheckInCommon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subscribebox : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserCheckIns", "subscribe", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserCheckIns", "subscribe");
        }
    }
}
