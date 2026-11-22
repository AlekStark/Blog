using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Content { get; set; }

    public int AuthorId { get; set; }
    public User Author { get; set; }

    public int ArticleId { get; set; }
    public Article Article { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}