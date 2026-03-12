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

    public async Task<bool> AddPreferitoAsync(Preferito preferito)
    {
        try
        {
            _context.Preferitis.Add(preferito);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task<bool> DeletePreferitoAsync(int idUtente, int tmdbId)
    {
        var preferito = await _context.Preferitis
            .FirstOrDefaultAsync(p => p.IdUtente == idUtente && p.TmdbId == tmdbId);

        if (preferito == null)
        {
            return false;
        }

        _context.Preferitis.Remove(preferito);

        var affectedRows = await _context.SaveChangesAsync();
        return affectedRows > 0;
    }

    public async Task<List<string>> GetGeneriTopByUtenteAsync(int idUtente, int quanti = 3)
    {
        var preferiti = await _context.Preferitis
            .Where(p => p.IdUtente == idUtente)
            .Select(p => p.GenereCache)
            .ToListAsync();

        if (!preferiti.Any()) return new List<string>();

        return preferiti
            .Where(g => !string.IsNullOrEmpty(g))
            .SelectMany(g => g!.Split(','))
            .GroupBy(g => g.Trim())
            .OrderByDescending(group => group.Count())
            .Take(quanti) // Ne prendiamo N invece di uno solo
            .Select(group => group.Key)
            .ToList();
    }


}