using Microsoft.AspNetCore.Mvc;
using CineBit.DTOs;
using CineBit.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PreferitiController : ControllerBase
{
    private readonly IPreferitiRepository _repository;
    private readonly TmdbService _tmdbService;

    public PreferitiController(IPreferitiRepository repository, TmdbService tmdbService)
    {
        _repository = repository;
        _tmdbService = tmdbService;
    }

    [HttpGet("utente/{idUtente}")]
    public async Task<ActionResult<IEnumerable<PreferitoDto>>> GetPreferiti(int idUtente)
    {
        var preferiti = await _repository.GetPreferitiByUtenteAsync(idUtente);
        var preferitiDto = preferiti.Select(p => new PreferitoDto
        {
            IdPrefe = p.IdPrefe,
            TmdbId = p.TmdbId,
            TitoloCache = p.TitoloCache,
            PosterPathCache = p.PosterPathCache,
            GenereCache = p.GenereCache,
            DataAggiunta = p.DataAggiunta,
            IdUtente = p.IdUtente
        });
        return Ok(preferitiDto);
    }

    [HttpPost]
    public async Task<IActionResult> AggiungiPreferito([FromBody] PreferitoDto dto)
    {
        if (dto == null) return BadRequest("Dati non validi");
        var nuovoPreferito = new Preferito
        {
            IdUtente = dto.IdUtente,
            TmdbId = dto.TmdbId,
            TitoloCache = dto.TitoloCache,
            PosterPathCache = dto.PosterPathCache,
            GenereCache = dto.GenereCache,
            DataAggiunta = DateTime.Now
        };
        var successo = await _repository.AddPreferitoAsync(nuovoPreferito);
        if (!successo) return Conflict(new { message = "Gia presente!" });
        return Ok(new { message = "Aggiunto!" });
    }

    [HttpDelete("utente/{idUtente}/film/{tmdbId}")]
    public async Task<IActionResult> RimuoviPreferito(int idUtente, int tmdbId)
    {
        var successo = await _repository.DeletePreferitoAsync(idUtente, tmdbId);
        if (!successo) return NotFound();
        return Ok(new { message = "Rimosso!" });
    }

    [HttpGet("utente/{idUtente}/consigliati")]
    public async Task<ActionResult<IEnumerable<PreferitoDto>>> GetConsigliati(int idUtente)
    {
        var generiTop = await _repository.GetGeneriTopByUtenteAsync(idUtente, 2);

        var mappaGeneri = new Dictionary<string, string> {
        { "Azione", "28" }, { "Thriller", "53" }, { "Avventura", "12" },
        { "Animazione", "16" }, { "Commedia", "35" }, { "Dramma", "18" },
        { "Fantascienza", "878" }, { "Horror", "27" }
    };

        var idsTrovati = generiTop
            .Where(g => mappaGeneri.ContainsKey(g))
            .Select(g => mappaGeneri[g])
            .ToList();

        string generiQuery = idsTrovati.Any() ? string.Join(",", idsTrovati) : "28";

        var parametri = new AiParameters { GenreId = generiQuery };
        var filmDaTmdb = await _tmdbService.SearchMoviesAsync(parametri);

        var preferiti = await _repository.GetPreferitiByUtenteAsync(idUtente);
        var idsGiaSalvati = preferiti.Select(p => p.TmdbId).ToHashSet();

        var consigliati = filmDaTmdb
            .Where(f => !idsGiaSalvati.Contains(f.Id))
            .Select(f => new PreferitoDto
            {
                TmdbId = f.Id,
                TitoloCache = f.Title,
                PosterPathCache = f.PosterPath,
                IdUtente = idUtente
            })
            .Take(6).ToList();

        return Ok(consigliati);
    }
}