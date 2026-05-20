using Microsoft.EntityFrameworkCore;
using JokesApi.Models;

namespace JokesApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }
    
    public DbSet<Joke> Jokes { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Joke>()
            .HasOne(j => j.Category)
            .WithMany(c => c.Jokes)
            .HasForeignKey(j => j.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Joke>().HasIndex(j => j.CategoryId);
        modelBuilder.Entity<Joke>().HasIndex(j => j.CreatedAt);
        
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Евгений и работа", Color = "#e74c3c", CreatedAt = DateTime.UtcNow },
            new Category { Id = 2, Name = "Евгений и учёба", Color = "#3498db", CreatedAt = DateTime.UtcNow },
            new Category { Id = 3, Name = "Евгений и программирование", Color = "#2ecc71", CreatedAt = DateTime.UtcNow },
            new Category { Id = 4, Name = "Евгений и жизнь", Color = "#f39c12", CreatedAt = DateTime.UtcNow }
        );
        
        modelBuilder.Entity<Joke>().HasData(
            new Joke { Id = 1, Title = "Евгений приходит в бар, а бармен говорит: «Опять?»", Rating = 5, CategoryId = 4, IsPinned = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Joke { Id = 2, Title = "Евгений говорит: «Я фронтенд выучу». Все засмеялись...", Rating = 5, CategoryId = 2, IsPinned = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Joke { Id = 3, Title = "— Почему Евгений не боится дедлайнов? — Потому что он сам — дедлайн.", Rating = 4, CategoryId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Joke { Id = 4, Title = "Евгений объясняет CORS на пальцах кота", Rating = 5, CategoryId = 3, IsPinned = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Joke { Id = 5, Title = "Старый архивный анекдот", Rating = 2, CategoryId = 4, IsArchived = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );
    }
}