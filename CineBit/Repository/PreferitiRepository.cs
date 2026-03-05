using Microsoft.EntityFrameworkCore;
using CineBit.Models;

public class PreferitiRepository : IPreferitiRepository
{
    private readonly CinebitDbContext _context;

    public PreferitiRepository(CinebitDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Preferito>> GetPreferitiByUtenteAsync(int idUtente)
    {
        return await _context.Preferitis
            .Where(p => p.IdUtente == idUtente)
            .OrderByDescending(p => p.DataAggiunta)
            .ToListAsync();
    }


}