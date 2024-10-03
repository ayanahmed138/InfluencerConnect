namespace InfluencerConnect.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CampaignMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Campaigns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        TargetAudienceId = c.Int(nullable: false),
                        CatagoryId = c.Int(nullable: false),
                        CampaignMessageId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Category_Id = c.Int(),
                        Influencer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CampaignMessages", t => t.CampaignMessageId, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .ForeignKey("dbo.TargetAudiences", t => t.TargetAudienceId, cascadeDelete: true)
                .ForeignKey("dbo.Influencers", t => t.Influencer_Id)
                .Index(t => t.TargetAudienceId)
                .Index(t => t.CampaignMessageId)
                .Index(t => t.Category_Id)
                .Index(t => t.Influencer_Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TargetAudiences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ContentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Influencers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Name = c.String(),
                        ContactInfo = c.String(),
                        MinCharge = c.Int(nullable: false),
                        MaxCharge = c.Int(nullable: false),
                        Limit = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InfluencerContentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InfluencerId = c.Int(nullable: false),
                        ContentTypeId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContentTypes", t => t.ContentTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Influencers", t => t.InfluencerId, cascadeDelete: true)
                .Index(t => t.InfluencerId)
                .Index(t => t.ContentTypeId);
            
            CreateTable(
                "dbo.MarketingAgents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Name = c.String(),
                        Company = c.String(),
                        ContactInfo = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CampaignMsgId = c.Int(nullable: false),
                        InfluencerId = c.Int(nullable: false),
                        text = c.String(),
                        title = c.String(),
                        isAccepted = c.Boolean(),
                        isDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InfluencerContentTypes", "InfluencerId", "dbo.Influencers");
            DropForeignKey("dbo.InfluencerContentTypes", "ContentTypeId", "dbo.ContentTypes");
            DropForeignKey("dbo.Campaigns", "Influencer_Id", "dbo.Influencers");
            DropForeignKey("dbo.Campaigns", "TargetAudienceId", "dbo.TargetAudiences");
            DropForeignKey("dbo.Campaigns", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.Campaigns", "CampaignMessageId", "dbo.CampaignMessages");
            DropIndex("dbo.InfluencerContentTypes", new[] { "ContentTypeId" });
            DropIndex("dbo.InfluencerContentTypes", new[] { "InfluencerId" });
            DropIndex("dbo.Campaigns", new[] { "Influencer_Id" });
            DropIndex("dbo.Campaigns", new[] { "Category_Id" });
            DropIndex("dbo.Campaigns", new[] { "CampaignMessageId" });
            DropIndex("dbo.Campaigns", new[] { "TargetAudienceId" });
            DropTable("dbo.Notifications");
            DropTable("dbo.MarketingAgents");
            DropTable("dbo.InfluencerContentTypes");
            DropTable("dbo.Influencers");
            DropTable("dbo.ContentTypes");
            DropTable("dbo.TargetAudiences");
            DropTable("dbo.Categories");
            DropTable("dbo.Campaigns");
            DropTable("dbo.CampaignMessages");
        }
    }
}
