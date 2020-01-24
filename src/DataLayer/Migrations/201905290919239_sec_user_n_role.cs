namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sec_user_n_role : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sec_Role",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                        RoleDescriptor = c.String(),
                        Sec_User_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.RoleId)
                .ForeignKey("dbo.Sec_User", t => t.Sec_User_UserId)
                .Index(t => t.Sec_User_UserId);
            
            CreateTable(
                "dbo.Sec_User",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Alias = c.String(),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sec_Role", "Sec_User_UserId", "dbo.Sec_User");
            DropIndex("dbo.Sec_Role", new[] { "Sec_User_UserId" });
            DropTable("dbo.Sec_User");
            DropTable("dbo.Sec_Role");
        }
    }
}
