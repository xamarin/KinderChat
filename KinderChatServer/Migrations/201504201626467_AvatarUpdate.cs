namespace KinderChatServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AvatarUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Avatars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Location = c.String(),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Users", "Avatar_Id", c => c.Int());
            CreateIndex("dbo.Users", "Avatar_Id");
            AddForeignKey("dbo.Users", "Avatar_Id", "dbo.Avatars", "Id");
            DropColumn("dbo.Users", "Avatar");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Avatar", c => c.String());
            DropForeignKey("dbo.Users", "Avatar_Id", "dbo.Avatars");
            DropIndex("dbo.Users", new[] { "Avatar_Id" });
            DropColumn("dbo.Users", "Avatar_Id");
            DropTable("dbo.Avatars");
        }
    }
}
