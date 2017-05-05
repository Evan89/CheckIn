namespace CheckIn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class secNumadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserCheckIns", "secNum", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserCheckIns", "secNum");
        }
    }
}
