namespace DataBase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTableNameToTableEntities : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TableEntity", newName: "TableEntities");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.TableEntities", newName: "TableEntity");
        }
    }
}
