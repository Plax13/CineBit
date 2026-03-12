using CineBit.Models;

public interface IPreferitiRepository
{
    Task<IEnumerable<Preferito>> GetPreferitiByUtenteAsync(int idUtente);

    Task<bool> AddPreferitoAsync(Preferito preferito);

    Task<bool> DeletePreferitoAsync(int idUtente, int tmdbId);

    Task<List<string>> GetGeneriTopByUtenteAsync(int idUtente, int quanti = 3);
}