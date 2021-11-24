namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrationForVoltage : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TableEntities", newName: "TableEntity");
            AddColumn("dbo.TableEntity", "Voltage", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TableEntity", "Voltage");
            RenameTable(name: "dbo.TableEntity", newName: "TableEntities");
        }
    }
}
