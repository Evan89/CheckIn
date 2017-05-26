namespace CheckInCommon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class int2long : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserCheckIns");
            AddColumn("dbo.UserCheckIns", "ID", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.UserCheckIns", "secString", c => c.String());
            AddPrimaryKey("dbo.UserCheckIns", "ID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.UserCheckIns");
            AlterColumn("dbo.UserCheckIns", "secString", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.UserCheckIns", "ID");
            AddPrimaryKey("dbo.UserCheckIns", "secString");
        }
    }
}
