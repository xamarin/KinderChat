namespace KinderChatServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Flags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Flags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        AlertLevel = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserFlags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FlagId = c.Int(nullable: false),
                        Resolved = c.Boolean(nullable: false),
                        AccusedUserId = c.Int(nullable: false),
                        AccuserUserId = c.Int(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .ForeignKey("dbo.Users", t => t.AccusedUserId, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.AccuserUserId, cascadeDelete: false)
                .ForeignKey("dbo.Flags", t => t.FlagId, cascadeDelete: false)
                .Index(t => t.FlagId)
                .Index(t => t.AccusedUserId)
                .Index(t => t.AccuserUserId)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserFlags", "FlagId", "dbo.Flags");
            DropForeignKey("dbo.UserFlags", "AccuserUserId", "dbo.Users");
            DropForeignKey("dbo.UserFlags", "AccusedUserId", "dbo.Users");
            DropForeignKey("dbo.UserFlags", "User_Id", "dbo.Users");
            DropIndex("dbo.UserFlags", new[] { "User_Id" });
            DropIndex("dbo.UserFlags", new[] { "AccuserUserId" });
            DropIndex("dbo.UserFlags", new[] { "AccusedUserId" });
            DropIndex("dbo.UserFlags", new[] { "FlagId" });
            DropTable("dbo.UserFlags");
            DropTable("dbo.Flags");
        }
    }
}
