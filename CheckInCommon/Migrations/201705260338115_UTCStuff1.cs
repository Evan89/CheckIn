namespace CheckInCommon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UTCStuff1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserCheckIns", "inputTime", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserCheckIns", "inputTime", c => c.String(nullable: false));
        }
    }
}
