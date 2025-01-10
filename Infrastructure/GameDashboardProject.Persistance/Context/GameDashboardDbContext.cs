using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GameDashboardProject.Domain.Identities;
using GameDashboardProject.Domain.Buildings;
using MongoDB.Bson;

namespace GameDashboardProject.Persistence.Context
{
    public class GameDashboardDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public GameDashboardDbContext(DbContextOptions<GameDashboardDbContext> options) : base(options) { }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Building> Buildings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppRole>().HasData(
                new AppRole { Id = new Guid("0fb7942c-c095-4e2a-aa08-96e866718ead"), Name = "Admin", NormalizedName = "ADMIN" },
                new AppRole { Id = new Guid("c307dd37-01be-46fe-b8c5-b6c1cfd6806d"), Name = "User", NormalizedName = "USER" }
            );

            builder.Entity<AppUser>().HasIndex(u => u.UserName).IsUnique();
            builder.Entity<AppUser>().HasIndex(u => u.Email).IsUnique();

            builder.Entity<Building>()
                .Property(b => b.Id)
                .HasConversion(
                    id => id.ToString(), 
                    id => string.IsNullOrEmpty(id) ? ObjectId.Empty : ObjectId.Parse(id));  

            builder.Entity<Building>().HasKey(b => b.Id);
        }
    }
}
