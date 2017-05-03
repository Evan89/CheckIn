namespace CheckIn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailtest : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserCheckIns");
            AddColumn("dbo.UserCheckIns", "ID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.UserCheckIns", "email", c => c.String(nullable: false));
            AddPrimaryKey("dbo.UserCheckIns", "ID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.UserCheckIns");
            AlterColumn("dbo.UserCheckIns", "email", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.UserCheckIns", "ID");
            AddPrimaryKey("dbo.UserCheckIns", "email");
        }
    }
}
