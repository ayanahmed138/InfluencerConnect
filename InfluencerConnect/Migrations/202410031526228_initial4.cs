namespace InfluencerConnect.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InfluencerContentTypes", "InfluencerId", "dbo.Influencers");
            DropIndex("dbo.InfluencerContentTypes", new[] { "InfluencerId" });
            AddColumn("dbo.InfluencerContentTypes", "Influencer_Id", c => c.Int());
            AlterColumn("dbo.InfluencerContentTypes", "InfluencerId", c => c.String());
            CreateIndex("dbo.InfluencerContentTypes", "Influencer_Id");
            AddForeignKey("dbo.InfluencerContentTypes", "Influencer_Id", "dbo.Influencers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InfluencerContentTypes", "Influencer_Id", "dbo.Influencers");
            DropIndex("dbo.InfluencerContentTypes", new[] { "Influencer_Id" });
            AlterColumn("dbo.InfluencerContentTypes", "InfluencerId", c => c.Int(nullable: false));
            DropColumn("dbo.InfluencerContentTypes", "Influencer_Id");
            CreateIndex("dbo.InfluencerContentTypes", "InfluencerId");
            AddForeignKey("dbo.InfluencerContentTypes", "InfluencerId", "dbo.Influencers", "Id", cascadeDelete: true);
        }
    }
}
