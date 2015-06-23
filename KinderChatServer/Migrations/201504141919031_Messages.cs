namespace KinderChatServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Messages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ToUserId = c.Int(nullable: false),
                        FromUserId = c.Int(nullable: false),
                        DeviceId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                        EncryptedMessage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Messages");
        }
    }
}
