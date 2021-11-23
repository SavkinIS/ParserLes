namespace ParserLes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upDateData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParserSettings", "NumberPage", c => c.Int(nullable: false));
            AddColumn("dbo.ParserSettings", "ProductsCount", c => c.Int(nullable: false));
            DropColumn("dbo.ParserSettings", "LastCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ParserSettings", "LastCount", c => c.Int(nullable: false));
            DropColumn("dbo.ParserSettings", "ProductsCount");
            DropColumn("dbo.ParserSettings", "NumberPage");
        }
    }
}
