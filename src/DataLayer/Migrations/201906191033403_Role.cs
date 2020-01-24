namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Role : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sec_Role", "Sec_User_UserId", "dbo.Sec_User");
            DropIndex("dbo.Sec_Role", new[] { "Sec_User_UserId" });
            AddColumn("dbo.Sec_User", "AuthorizationLevel", c => c.Int(nullable: false));
            DropColumn("dbo.Sec_Role", "Sec_User_UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sec_Role", "Sec_User_UserId", c => c.Int());
            DropColumn("dbo.Sec_User", "AuthorizationLevel");
            CreateIndex("dbo.Sec_Role", "Sec_User_UserId");
            AddForeignKey("dbo.Sec_Role", "Sec_User_UserId", "dbo.Sec_User", "UserId");
        }
    }
}
