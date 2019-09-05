using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Confortex.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Tenga en cuenta que el valor de authenticationType debe coincidir con el definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Agregar aquí notificaciones personalizadas de usuario
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>().ToTable("seg.Usuario").Property(p => p.Id).HasColumnName("IdUsuario");
            modelBuilder.Entity<IdentityUser>().ToTable("seg.Usuario").Property(p => p.PasswordHash).HasColumnName("Password");

            modelBuilder.Entity<IdentityRole>().ToTable("seg.Rol");
            modelBuilder.Entity<IdentityUserRole>().ToTable("seg.UsuarioRol");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("seg.UsuarioClaim");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("seg.UsuarioLogin");

        }
    }
}