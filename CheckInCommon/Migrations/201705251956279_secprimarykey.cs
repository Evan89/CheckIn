namespace CheckInCommon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class secprimarykey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserCheckIns");
            AlterColumn("dbo.UserCheckIns", "secString", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.UserCheckIns", "secString");
            DropColumn("dbo.UserCheckIns", "ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserCheckIns", "ID", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.UserCheckIns");
            AlterColumn("dbo.UserCheckIns", "secString", c => c.String());
            AddPrimaryKey("dbo.UserCheckIns", "ID");
        }
    }
}
