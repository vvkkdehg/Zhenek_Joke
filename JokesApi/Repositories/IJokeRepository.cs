using JokesApi.Models;
using JokesApi.Models.DTOs;

namespace JokesApi.Repositories;

public interface IJokeRepository
{
    Task<IEnumerable<JokeResponseDto>> GetAllAsync(bool archived = false);
    Task<JokeResponseDto?> GetByIdAsync(int id);
    Task<Joke> CreateAsync(Joke joke);
    Task<Joke> UpdateAsync(Joke joke);
    Task DeleteAsync(Joke joke);
    Task<Joke?> FindAsync(int id);
}