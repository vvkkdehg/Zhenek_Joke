using System.ComponentModel.DataAnnotations;

namespace JokesApi.Models.DTOs;

public class CreateJokeDto
{
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;
    
    [Range(1, 5)]
    public int Rating { get; set; } = 3;
    
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;
    
    [Required]
    public int CategoryId { get; set; }
}