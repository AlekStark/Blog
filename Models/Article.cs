using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models;

public class Article
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public int AuthorId { get; set; }
    public User Author { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ArticleTag> ArticleTags { get; set; }
    public ICollection<Comment> Comments { get; set; }
}