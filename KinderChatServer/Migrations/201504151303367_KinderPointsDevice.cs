namespace KinderChatServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KinderPointsDevice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserDevices", "KinderPoints", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserDevices", "KinderPoints");
        }
    }
}
