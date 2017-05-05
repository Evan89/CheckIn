namespace CheckIn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rndNumadd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserCheckIns", "rndNum", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserCheckIns", "rndNum");
        }
    }
}
