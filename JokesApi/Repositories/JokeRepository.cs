using Microsoft.EntityFrameworkCore;
using JokesApi.Data;
using JokesApi.Models;
using JokesApi.Models.DTOs;

namespace JokesApi.Repositories;

public class JokeRepository : IJokeRepository
{
    private readonly AppDbContext _db;
    
    public JokeRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<IEnumerable<JokeResponseDto>> GetAllAsync(bool archived = false)
    {
        return await _db.Jokes
            .Include(j => j.Category)
            .Where(j => j.IsArchived == archived)
            .OrderByDescending(j => j.IsPinned)
            .ThenByDescending(j => j.CreatedAt)
            .Select(j => new JokeResponseDto
            {
                Id = j.Id,
                Title = j.Title,
                Content = j.Content,
                Rating = j.Rating,
                ImageUrl = j.ImageUrl,
                IsPinned = j.IsPinned,
                IsArchived = j.IsArchived,
                CreatedAt = j.CreatedAt,
                UpdatedAt = j.UpdatedAt,
                CategoryId = j.CategoryId,
                CategoryName = j.Category.Name,
                CategoryColor = j.Category.Color
            })
            .ToListAsync();
    }
    
    public async Task<JokeResponseDto?> GetByIdAsync(int id)
    {
        return await _db.Jokes
            .Include(j => j.Category)
            .Where(j => j.Id == id)
            .Select(j => new JokeResponseDto
            {
                Id = j.Id,
                Title = j.Title,
                Content = j.Content,
                Rating = j.Rating,
                ImageUrl = j.ImageUrl,
                IsPinned = j.IsPinned,
                IsArchived = j.IsArchived,
                CreatedAt = j.CreatedAt,
                UpdatedAt = j.UpdatedAt,
                CategoryId = j.CategoryId,
                CategoryName = j.Category.Name,
                CategoryColor = j.Category.Color
            })
            .FirstOrDefaultAsync();
    }
    
    public async Task<Joke> CreateAsync(Joke joke)
    {
        _db.Jokes.Add(joke);
        await _db.SaveChangesAsync();
        return joke;
    }
    
    public async Task<Joke> UpdateAsync(Joke joke)
    {
        joke.UpdatedAt = DateTime.UtcNow;
        _db.Jokes.Update(joke);
        await _db.SaveChangesAsync();
        return joke;
    }
    
    public async Task DeleteAsync(Joke joke)
    {
        _db.Jokes.Remove(joke);
        await _db.SaveChangesAsync();
    }
    
    public async Task<Joke?> FindAsync(int id)
    {
        return await _db.Jokes.FindAsync(id);
    }
}