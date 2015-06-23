namespace KinderChatServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Joined : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Joined", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Joined");
        }
    }
}
