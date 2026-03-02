using CineBit.Models;

public class UserService : IUserService
{
    private readonly IUtentiRepo _userRepository;

    public UserService(IUtentiRepo userRepository)
    {
        _userRepository = userRepository;

    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
            throw new Exception("Email già registrata");

        var user = new Utente
        {
            Nome = request.Nome,
            Cognome = request.Cognome,
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Ruolo = "Utente"
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

    }

    public async Task LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new UnauthorizedAccessException("Credenziali non valide");

        var validPassword = BCrypt.Net.BCrypt
            .Verify(request.Password, user.Password);

        if (!validPassword)
            throw new UnauthorizedAccessException("Credenziali non valide");


    }
}