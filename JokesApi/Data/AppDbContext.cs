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
            new Joke { Id = 1, Title = "Идут по лесу Белоснежка, Дюймовочка и Самый плохой программист. Белоснежка говорит: \n— Я самая красивая./nДюймовочка говорит:\n— Я самая маленькая.\nПрограммист говорит:\n— Я больше всех дедлайнов запорол.\nТут они заходят в дом правды. Выбегает оттуда в слезах Белоснежка:\n— Я не самая красивая, самая красивая-Спящая Красавица!.\nВыходит расстроенная Дюймовочка:\n— Я не самая Маленькая, Мальчик-c-Пальчик меньше меня!\nВыходит разъяренный программист и как заорёт:\n— КТО ТАКОЙ ЕВГЕНИЙ ЖУРАВСКИЙ?!", Rating = 5, CategoryId = 3, IsPinned = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Joke { Id = 2, Title = "Друг-американец достает из холодильника последнюю бутылку водки и говорит Женьку:\n– Will уou?\n– Я те, гад, вылью!", Rating = 5, CategoryId = 4, IsPinned = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Joke { Id = 3, Title = "На вечеринке Женек подходит к красивой девушке и спрашивает:\n– Извините, вы уже приглашены на следующий танец?\n– Нет, я свободна.\n– Тогда не могли бы вы присмотреть за моим пивом?", Rating = 4, CategoryId = 4, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Joke { Id = 4, Title = "Евгений на экзамене молчит и не отвечает на первый вопрос билета.\n- Хорошо, - говорит препод через минуту. - Отвечайте на второй вопрос!\n- А на второй вопрос, профессор, я знаю ответ ещё хуже!", Rating = 5, CategoryId = 2, IsPinned = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Joke { Id = 5, Title = "Евгений пришёл устраиваться на работу. На собеседовании директор фирмы его спрашивает:\n– Расскажите о себе в двух словах.\n– Всякое бывало…", Rating = 2, CategoryId = 1, IsArchived = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );
    }
}