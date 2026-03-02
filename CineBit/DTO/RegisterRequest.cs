using System.ComponentModel.DataAnnotations;

public record RegisterRequest
{
    [Required]
    [MinLength(2)]
    public string Nome { get; init; } = null!;

    [Required]
    [MinLength(2)]
    public string Cognome { get; init; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = null!;

    [Required]
    [MinLength(8)]
    public string Password { get; init; } = null!;
}

