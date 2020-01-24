namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClientInfo",
                c => new
                    {
                        ClientInfoId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        NodeName = c.String(),
                        MAC = c.String(),
                        CPULoad = c.Int(nullable: false),
                        RamLoad = c.Int(nullable: false),
                        ProcId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ClientInfoId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ClientInfo");
        }
    }
}
