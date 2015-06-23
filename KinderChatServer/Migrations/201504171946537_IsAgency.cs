namespace KinderChatServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsAgency : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserDevices", "AgencyId", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "IsAgency", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "IsAgency");
            DropColumn("dbo.UserDevices", "AgencyId");
        }
    }
}
