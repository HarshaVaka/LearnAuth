using AuthMicroService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthMicroService.Infrastructure
{
    public class AuthServiceDBContext(DbContextOptions<AuthServiceDBContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRoleMapping> UserRoles { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey("UserId");
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.UserName).IsUnique();
            });

            modelBuilder.Entity<UserRoleMapping>(entity =>
            {
                entity.HasKey("UserId", "RoleId");
            });

            modelBuilder.Entity<UserRoleMapping>(entity =>
            {
                entity.HasOne(u => u.User)
                .WithMany(urm => urm.UserRoles)
                .HasForeignKey(u => u.UserId);
            });
            modelBuilder.Entity<UserRoleMapping>(entity =>
            {
                entity.HasOne(r => r.Role)
                .WithMany(urm => urm.UserRoles)
                .HasForeignKey(r => r.RoleId);
            });
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(rt => rt.UserId); // Index on UserId
            });
        }
    }
}
