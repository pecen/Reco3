namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VectoApiVersion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VSumRecord", "VectoApiVersion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VSumRecord", "VectoApiVersion");
        }
    }
}
