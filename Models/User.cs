using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Login { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    [MaxLength(100)]
    public string Email { get; set; }

    public ICollection<Article> Articles { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}