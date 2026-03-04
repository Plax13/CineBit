using CineBit.Models;

public interface IUtentiRepo : IRepository<Utenti>
{
    Task<Utenti?> GetByEmailAsync(string email);
}