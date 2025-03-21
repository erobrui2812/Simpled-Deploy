using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskBoard.api.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskBoard.api.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Board> Boards { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<BoardMember> BoardMembers { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Activity> Activities { get; set; } // Añadir este DbSet

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuración para SQLite
            builder.Entity<Item>()
                .Property(i => i.DueDate)
                .HasConversion(
                    v => v.ToUniversalTime().ToString("O"),
                    v => DateTime.Parse(v).ToUniversalTime());

            builder.Entity<BoardMember>()
                .HasKey(bm => new { bm.BoardId, bm.UserId });

            // Configurar relación con Board
            builder.Entity<Invitation>()
                .HasOne(i => i.Board)
                .WithMany()
                .HasForeignKey(i => i.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración adicional para Activity
            builder.Entity<Activity>(entity =>
            {
                entity.HasIndex(a => a.BoardId);
                entity.HasIndex(a => a.UserId);
                entity.Property(a => a.Details).HasColumnType("TEXT");
            });
        }
    }
}
