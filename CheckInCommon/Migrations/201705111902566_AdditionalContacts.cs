namespace CheckInCommon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdditionalContacts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserCheckIns", "contactEmail1", c => c.String(nullable: false));
            AddColumn("dbo.UserCheckIns", "contactEmail2", c => c.String());
            AddColumn("dbo.UserCheckIns", "contactEmail3", c => c.String());
            AddColumn("dbo.UserCheckIns", "contactEmail4", c => c.String());
            DropColumn("dbo.UserCheckIns", "contactEmail");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserCheckIns", "contactEmail", c => c.String(nullable: false));
            DropColumn("dbo.UserCheckIns", "contactEmail4");
            DropColumn("dbo.UserCheckIns", "contactEmail3");
            DropColumn("dbo.UserCheckIns", "contactEmail2");
            DropColumn("dbo.UserCheckIns", "contactEmail1");
        }
    }
}
