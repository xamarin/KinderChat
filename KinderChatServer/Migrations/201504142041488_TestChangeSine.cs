namespace KinderChatServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestChangeSine : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Messages", "DeviceId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Messages", "DeviceId", c => c.Int(nullable: false));
        }
    }
}
