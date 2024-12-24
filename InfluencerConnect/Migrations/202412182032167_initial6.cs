namespace InfluencerConnect.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CampaignMessages", "CampaignId", c => c.Int(nullable: false));
            AddColumn("dbo.Campaigns", "IsPrivate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Campaigns", "IsPrivate");
            DropColumn("dbo.CampaignMessages", "CampaignId");
        }
    }
}
