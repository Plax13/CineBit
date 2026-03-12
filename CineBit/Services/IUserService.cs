using CineBit.Models;

public interface IUserService
{
    Task RegisterAsync(RegisterRequest request);
    Task<Utenti> LoginAsync(LoginRequest request);
}