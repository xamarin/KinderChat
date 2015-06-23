namespace KinderChatServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class flags6 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.UserFlags", "FlagId");
            CreateIndex("dbo.UserFlags", "AccusedUserId");
            CreateIndex("dbo.UserFlags", "AccuserUserId");
            AddForeignKey("dbo.UserFlags", "AccusedUserId", "dbo.Users", "Id", cascadeDelete: false);
            AddForeignKey("dbo.UserFlags", "AccuserUserId", "dbo.Users", "Id", cascadeDelete: false);
            AddForeignKey("dbo.UserFlags", "FlagId", "dbo.Flags", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserFlags", "FlagId", "dbo.Flags");
            DropForeignKey("dbo.UserFlags", "AccuserUserId", "dbo.Users");
            DropForeignKey("dbo.UserFlags", "AccusedUserId", "dbo.Users");
            DropIndex("dbo.UserFlags", new[] { "AccuserUserId" });
            DropIndex("dbo.UserFlags", new[] { "AccusedUserId" });
            DropIndex("dbo.UserFlags", new[] { "FlagId" });
        }
    }
}
