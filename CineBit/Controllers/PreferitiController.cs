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
}