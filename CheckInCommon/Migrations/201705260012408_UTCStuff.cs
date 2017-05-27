namespace CheckInCommon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UTCStuff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserCheckIns", "inputTime", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserCheckIns", "inputTime");
        }
    }
}
