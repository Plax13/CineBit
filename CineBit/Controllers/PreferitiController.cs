using Microsoft.AspNetCore.Mvc;
using CineBit.DTOs;
using CineBit.Models;

[ApiController]
[Route("api/[controller]")]
public class PreferitiController : ControllerBase
{
    private readonly IPreferitiRepository _repository;

    public PreferitiController(IPreferitiRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("utente/{idUtente}")]
    public async Task<ActionResult<IEnumerable<PreferitoDto>>> GetPreferiti(int idUtente)
    {
        var preferiti = await _repository.GetPreferitiByUtenteAsync(idUtente);
        
        // Mappatura da Model a DTO
        var preferitiDto = preferiti.Select(p => new PreferitoDto
        {
            IdPrefe = p.IdPrefe,
            TmdbId = p.TmdbId,
            TitoloCache = p.TitoloCache,
            DataAggiunta = p.DataAggiunta
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
            DataAggiunta = DateTime.Now
        };

        var successo = await _repository.AddPreferitoAsync(nuovoPreferito);

        if (!successo)
        {
            // Restituiamo un 409 Conflict se il film esiste già
            return Conflict(new { message = "Questo film è già nei tuoi preferiti!" });
        }

        return Ok(new { message = "Film aggiunto correttamente!" });
    }
}