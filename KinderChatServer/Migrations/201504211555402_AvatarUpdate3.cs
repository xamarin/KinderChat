namespace KinderChatServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AvatarUpdate3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "Avatar_Id", "dbo.Avatars");
            DropIndex("dbo.Users", new[] { "Avatar_Id" });
            RenameColumn(table: "dbo.Users", name: "Avatar_Id", newName: "AvatarId");
            AlterColumn("dbo.Users", "AvatarId", c => c.Int(nullable: false));
            CreateIndex("dbo.Users", "AvatarId");
            AddForeignKey("dbo.Users", "AvatarId", "dbo.Avatars", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "AvatarId", "dbo.Avatars");
            DropIndex("dbo.Users", new[] { "AvatarId" });
            AlterColumn("dbo.Users", "AvatarId", c => c.Int());
            RenameColumn(table: "dbo.Users", name: "AvatarId", newName: "Avatar_Id");
            CreateIndex("dbo.Users", "Avatar_Id");
            AddForeignKey("dbo.Users", "Avatar_Id", "dbo.Avatars", "Id");
        }
    }
}
