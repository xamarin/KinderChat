namespace KinderChatServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Achievements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Achievements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        BadgeLocation = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AchievementsRecieveds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AchievementId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Achievements", t => t.AchievementId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.AchievementId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Users", "NickName", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AchievementsRecieveds", "UserId", "dbo.Users");
            DropForeignKey("dbo.AchievementsRecieveds", "AchievementId", "dbo.Achievements");
            DropIndex("dbo.AchievementsRecieveds", new[] { "UserId" });
            DropIndex("dbo.AchievementsRecieveds", new[] { "AchievementId" });
            DropColumn("dbo.Users", "NickName");
            DropTable("dbo.AchievementsRecieveds");
            DropTable("dbo.Achievements");
        }
    }
}
