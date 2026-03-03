using CineBit.Models;
using Microsoft.EntityFrameworkCore;

public class UtentiRepository
    : Repository<Utenti>, IUtentiRepo
{
    public UtentiRepository(CinebitDbContext context)
        : base(context)
    {
    }

    public async Task<Utenti?> GetByEmailAsync(string email)
    {
        return await _dbset
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}