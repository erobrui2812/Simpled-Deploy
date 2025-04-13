using Microsoft.EntityFrameworkCore;
using Simpled.Models;


namespace Simpled.Data
{
    public class SimpledDbContext : DbContext
    {
        public SimpledDbContext(DbContextOptions<SimpledDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Board> Boards => Set<Board>();
        public DbSet<BoardColumn> BoardColumns => Set<BoardColumn>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<Content> Contents => Set<Content>();
        public DbSet<BoardMember> BoardMembers => Set<BoardMember>();

        public DbSet<BoardInvitation> BoardInvitations => Set<BoardInvitation>();

        public DbSet<UserAchievement> UserAchievements { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<Board>()
                .HasOne(b => b.Owner)
                .WithMany()
                .HasForeignKey(b => b.OwnerId);

            modelBuilder.Entity<BoardColumn>()
                .HasOne(c => c.Board)
                .WithMany(b => b.Columns)
                .HasForeignKey(c => c.BoardId);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.Column)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.ColumnId);

            modelBuilder.Entity<Content>()
                .HasOne(cnt => cnt.Item)
                .WithMany(i => i.Contents)
                .HasForeignKey(cnt => cnt.ItemId);


            modelBuilder.Entity<BoardMember>()
                .HasKey(bm => new { bm.BoardId, bm.UserId });

            modelBuilder.Entity<BoardMember>()
                .HasOne(bm => bm.Board)
                .WithMany(b => b.BoardMembers)
                .HasForeignKey(bm => bm.BoardId);

            modelBuilder.Entity<BoardMember>()
                .HasOne(bm => bm.User)
                .WithMany(u => u.BoardMembers)
                .HasForeignKey(bm => bm.UserId);

            modelBuilder.Entity<BoardInvitation>()
                .HasOne(i => i.Board)
                .WithMany()
                .HasForeignKey(i => i.BoardId);

            modelBuilder.Entity<UserAchievement>()
                .HasOne(a => a.User)
                .WithMany(u => u.Logros)
                .HasForeignKey(a => a.UserId);
        }

    }
}
