using System.ComponentModel.DataAnnotations;

namespace JokesApi.Models;

public class Joke
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;
    
    [Range(1, 5)]
    public int Rating { get; set; } = 3;
    
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;
    
    public bool IsPinned { get; set; } = false;
    public bool IsArchived { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}