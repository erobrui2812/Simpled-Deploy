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
        }

        public DbSet<Board> Boards { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<BoardMember> BoardMembers { get; set; }
        public DbSet<Content> Contents { get; set; }
    }
}

public class Board
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid OwnerId { get; set; }
    public bool IsPublic { get; set; }

    public User Owner { get; set; }
    public List<Column> Columns { get; set; } = new List<Column>();
    public List<BoardMember> Members { get; set; } = new List<BoardMember>();
}

public class BoardMember
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid BoardId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } // "admin|editor|viewer"

    public Board Board { get; set; }
    public User User { get; set; }
}

public class Column
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public Guid BoardId { get; set; }
    public string Title { get; set; }
    public int Order { get; set; }

    public Board Board { get; set; }
    public List<Item> Items { get; set; } = new List<Item>();
}

public class Item
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public Guid ColumnId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }

    public Column Column { get; set; }
    public List<Content> Contents { get; set; } = new List<Content>();
}

public class Content
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string Type { get; set; } // "text|image|checkbox"
    public string Value { get; set; }

    public Item Item { get; set; }
}

public class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Board> OwnedBoards { get; set; } = new List<Board>();
    public ICollection<BoardMember> MemberBoards { get; set; } = new List<BoardMember>();
}

