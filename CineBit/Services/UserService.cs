using CineBit.Models;
using Microsoft.AspNetCore.Identity; 

public class UserService : IUserService
{
    private readonly UserManager<Utente> _userManager;

    public UserService(UserManager<Utente> userManager)
    {
        _userManager = userManager;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLower();


        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
            throw new Exception("Email già registrata");

        var user = new Utente
        {
            Nome = request.Nome,
            Cognome = request.Cognome,
            UserName = email, 
            Email = email,
            Ruolo = Role.Utente
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Errore registrazione: {errors}");
        }
    }

    public async Task LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim().ToLower());

        if (user == null)
            throw new UnauthorizedAccessException("Credenziali non valide");


        var result = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!result)
            throw new UnauthorizedAccessException("Credenziali non valide");
    }
}
