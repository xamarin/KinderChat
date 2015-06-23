namespace KinderChatServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class flags5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserFlags", "Id", "dbo.Users");
            DropForeignKey("dbo.UserFlags", "FlagId", "dbo.Flags");
            DropIndex("dbo.UserFlags", new[] { "Id" });
            DropIndex("dbo.UserFlags", new[] { "FlagId" });
            DropPrimaryKey("dbo.UserFlags");
            AlterColumn("dbo.UserFlags", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.UserFlags", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.UserFlags");
            AlterColumn("dbo.UserFlags", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.UserFlags", "Id");
            CreateIndex("dbo.UserFlags", "FlagId");
            CreateIndex("dbo.UserFlags", "Id");
            AddForeignKey("dbo.UserFlags", "FlagId", "dbo.Flags", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UserFlags", "Id", "dbo.Users", "Id");
        }
    }
}
