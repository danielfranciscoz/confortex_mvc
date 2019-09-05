namespace Confortex.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "seg.Rol",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "seg.UsuarioRol",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("seg.Rol", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("seg.Usuario", t => t.IdentityUser_Id)
                .Index(t => t.RoleId)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "seg.Usuario",
                c => new
                    {
                        IdUsuario = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        Password = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.IdUsuario)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "seg.UsuarioClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("seg.Usuario", t => t.IdentityUser_Id)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "seg.UsuarioLogin",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("seg.Usuario", t => t.IdentityUser_Id)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        IdUsuario = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.IdUsuario)
                .ForeignKey("seg.Usuario", t => t.IdUsuario)
                .Index(t => t.IdUsuario);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "IdUsuario", "seg.Usuario");
            DropForeignKey("seg.UsuarioRol", "IdentityUser_Id", "seg.Usuario");
            DropForeignKey("seg.UsuarioLogin", "IdentityUser_Id", "seg.Usuario");
            DropForeignKey("seg.UsuarioClaim", "IdentityUser_Id", "seg.Usuario");
            DropForeignKey("seg.UsuarioRol", "RoleId", "seg.Rol");
            DropIndex("dbo.AspNetUsers", new[] { "IdUsuario" });
            DropIndex("seg.UsuarioLogin", new[] { "IdentityUser_Id" });
            DropIndex("seg.UsuarioClaim", new[] { "IdentityUser_Id" });
            DropIndex("seg.Usuario", "UserNameIndex");
            DropIndex("seg.UsuarioRol", new[] { "IdentityUser_Id" });
            DropIndex("seg.UsuarioRol", new[] { "RoleId" });
            DropIndex("seg.Rol", "RoleNameIndex");
            DropTable("dbo.AspNetUsers");
            DropTable("seg.UsuarioLogin");
            DropTable("seg.UsuarioClaim");
            DropTable("seg.Usuario");
            DropTable("seg.UsuarioRol");
            DropTable("seg.Rol");
        }
    }
}
