using CineBit.Models;

public interface IPreferitiRepository
{
    Task<IEnumerable<Preferito>> GetPreferitiByUtenteAsync(int idUtente);

    Task<bool> AddPreferitoAsync(Preferito preferito);
}