namespace CheckIn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserCheckIns",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        firstName = c.String(nullable: false, maxLength: 20),
                        lastName = c.String(nullable: false, maxLength: 20),
                        telNum = c.String(nullable: false),
                        email = c.String(nullable: false),
                        contactEmail = c.String(nullable: false),
                        location = c.String(nullable: false, maxLength: 20),
                        returnTime = c.DateTime(nullable: false),
                        message = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserCheckIns");
        }
    }
}
