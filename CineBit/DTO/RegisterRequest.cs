using CineBit.Models;
using System.ComponentModel.DataAnnotations;

public record RegisterRequest
{
    [Required]
    [MinLength(2)]
    public string Nome { get; set; } = null!;

    [Required]
    [MinLength(2)]
    public string Cognome { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = null!;

    public string UserName { get; set; }

    public Role Role { get; set; }
}

