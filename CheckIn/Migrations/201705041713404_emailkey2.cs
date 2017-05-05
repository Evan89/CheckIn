namespace CheckIn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailkey2 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserCheckIns");
            AlterColumn("dbo.UserCheckIns", "email", c => c.String(nullable: false));
            AlterColumn("dbo.UserCheckIns", "ID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.UserCheckIns", "ID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.UserCheckIns");
            AlterColumn("dbo.UserCheckIns", "ID", c => c.Int(nullable: false));
            AlterColumn("dbo.UserCheckIns", "email", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.UserCheckIns", "email");
        }
    }
}
