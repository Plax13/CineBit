public interface IUserService
{
    Task RegisterAsync(RegisterRequest request);
    Task LoginAsync(LoginRequest request);
}