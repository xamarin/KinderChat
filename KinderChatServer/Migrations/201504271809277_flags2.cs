namespace KinderChatServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class flags2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.UserFlags", new[] { "Id" });
            DropPrimaryKey("dbo.UserFlags");
            AlterColumn("dbo.UserFlags", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.UserFlags", "Id");
            CreateIndex("dbo.UserFlags", "Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserFlags", new[] { "Id" });
            DropPrimaryKey("dbo.UserFlags");
            AlterColumn("dbo.UserFlags", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.UserFlags", "Id");
            CreateIndex("dbo.UserFlags", "Id");
        }
    }
}
