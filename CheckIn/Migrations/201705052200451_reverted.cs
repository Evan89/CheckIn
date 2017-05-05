namespace CheckIn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reverted : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.UserCheckIns", "rndNum");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserCheckIns", "rndNum", c => c.String());
        }
    }
}
