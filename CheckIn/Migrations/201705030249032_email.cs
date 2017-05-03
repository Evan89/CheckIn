namespace CheckIn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class email : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserCheckIns");
            AlterColumn("dbo.UserCheckIns", "email", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.UserCheckIns", "email");
            DropColumn("dbo.UserCheckIns", "ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserCheckIns", "ID", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.UserCheckIns");
            AlterColumn("dbo.UserCheckIns", "email", c => c.String(nullable: false));
            AddPrimaryKey("dbo.UserCheckIns", "ID");
        }
    }
}
