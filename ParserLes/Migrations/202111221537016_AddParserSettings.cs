namespace ParserLes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddParserSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ParserSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ParserSettings");
        }
    }
}
