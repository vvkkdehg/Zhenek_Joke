using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JokesApi.Helpers;
using JokesApi.Models;
using JokesApi.Data;
using JokesApi.Services;
using JokesApi.Models.DTOs;
using JokesApi.Repositories;

namespace JokesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JokesController : ControllerBase
{
    private readonly IJokeRepository _jokeRepo;
    private readonly AppDbContext _db;
    
    public JokesController(IJokeRepository jokeRepo, AppDbContext db)
    {
        _jokeRepo = jokeRepo;
        _db = db;
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<JokeResponseDto>>>> GetAll([FromQuery] bool archived = false)
    {
        var jokes = await _jokeRepo.GetAllAsync(archived);
        return Ok(ApiResponse<IEnumerable<JokeResponseDto>>.Ok(jokes));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<JokeResponseDto>>> GetById(int id)
    {
        var joke = await _jokeRepo.GetByIdAsync(id);
        if (joke == null)
            return NotFound(ApiError.NotFound($"Анекдот с id={id} не найден"));
        
        return Ok(ApiResponse<JokeResponseDto>.Ok(joke));
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<JokeResponseDto>>> Create([FromForm] CreateJokeDto dto)
    {
        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
            return BadRequest(ApiError.BadRequest($"Категория с id={dto.CategoryId} не существует"));
        
        string? imageUrl = null;
        
        if (dto.ImageFile != null)
        {
            try
            {
                var fileService = HttpContext.RequestServices.GetRequiredService<IFileUploadService>();
                imageUrl = await fileService.SaveImageAsync(dto.ImageFile);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiError.BadRequest(ex.Message));
            }
        }
        
        if (string.IsNullOrEmpty(imageUrl))
        {
            imageUrl = $"https://picsum.photos/id/{new Random().Next(1, 200)}/200/150";
        }
        
        var joke = new Joke
        {
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            Rating = dto.Rating,
            ImageUrl = imageUrl,
            CategoryId = dto.CategoryId,
            IsPinned = false,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var created = await _jokeRepo.CreateAsync(joke);
        var response = await _jokeRepo.GetByIdAsync(created.Id);
        
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<JokeResponseDto>.Created(response!, "Анекдот создан"));
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<JokeResponseDto>>> Update(int id, [FromBody] UpdateJokeDto dto)
    {
        var joke = await _jokeRepo.FindAsync(id);
        if (joke == null)
            return NotFound(ApiError.NotFound($"Анекдот с id={id} не найден"));
        
        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
            return BadRequest(ApiError.BadRequest($"Категория с id={dto.CategoryId} не существует"));
        
        joke.Title = dto.Title.Trim();
        joke.Content = dto.Content.Trim();
        joke.Rating = dto.Rating;
        joke.ImageUrl = dto.ImageUrl;
        joke.CategoryId = dto.CategoryId;
        
        await _jokeRepo.UpdateAsync(joke);
        var response = await _jokeRepo.GetByIdAsync(id);
        
        return Ok(ApiResponse<JokeResponseDto>.Ok(response!, "Анекдот обновлён"));
    }
    
    [HttpPatch("{id}/pin")]
    public async Task<ActionResult<ApiResponse<JokeResponseDto>>> TogglePin(int id)
    {
        var joke = await _jokeRepo.FindAsync(id);
        if (joke == null)
            return NotFound(ApiError.NotFound($"Анекдот с id={id} не найден"));
        
        joke.IsPinned = !joke.IsPinned;
        await _jokeRepo.UpdateAsync(joke);
        
        var response = await _jokeRepo.GetByIdAsync(id);
        var message = joke.IsPinned ? "Анекдот закреплён" : "Анекдот откреплён";
        
        return Ok(ApiResponse<JokeResponseDto>.Ok(response!, message));
    }
    
    [HttpPatch("{id}/archive")]
    public async Task<ActionResult<ApiResponse<JokeResponseDto>>> ToggleArchive(int id)
    {
        var joke = await _jokeRepo.FindAsync(id);
        if (joke == null)
            return NotFound(ApiError.NotFound($"Анекдот с id={id} не найден"));
        
        joke.IsArchived = !joke.IsArchived;
        if (joke.IsArchived)
            joke.IsPinned = false;
        
        await _jokeRepo.UpdateAsync(joke);
        
        var response = await _jokeRepo.GetByIdAsync(id);
        var message = joke.IsArchived ? "Анекдот архивирован" : "Анекдот восстановлен";
        
        return Ok(ApiResponse<JokeResponseDto>.Ok(response!, message));
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var joke = await _jokeRepo.FindAsync(id);
        if (joke == null)
            return NotFound(ApiError.NotFound($"Анекдот с id={id} не найден"));
        
        await _jokeRepo.DeleteAsync(joke);
        return NoContent();
    }
}