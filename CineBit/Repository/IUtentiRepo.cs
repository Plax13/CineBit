using CineBit.Models;

public interface IUtentiRepo : IRepository<Utente>
{
    Task<Utente?> GetByEmailAsync(string email);
}