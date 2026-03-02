using CineBit.Models;
using Microsoft.EntityFrameworkCore;

public class UtentiRepository
    : Repository<Utente>, IUtentiRepo
{
    public UtentiRepository(CinebitDbContext context)
        : base(context)
    {
    }

    public async Task<Utente?> GetByEmailAsync(string email)
    {
        return await _dbset
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}