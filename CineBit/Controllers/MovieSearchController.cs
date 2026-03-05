using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Json;
using System;



    [ApiController]
    [Route("api/[controller]")]
    public class MovieSearchController : ControllerBase
    {
        private readonly AiService _aiService;
        private readonly TmdbService _tmdbService;

        public MovieSearchController(AiService aiService, TmdbService tmdbService)
        {
            _aiService = aiService;
            _tmdbService = tmdbService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return BadRequest("La query non può essere vuota.");
            }

            try
            {
                // 1. Chiedi a Gemma di estrarre i parametri
                var aiParams = await _aiService.GetParamsFromGemmaAsync(request.Query);

                if (aiParams == null)
                {
                    return StatusCode(500, "Errore nell'analisi della richiesta da parte dell'AI.");
                }

                // 2. Usa i parametri per cercare su TMDB
                var movies = await _tmdbService.SearchMoviesAsync(aiParams);
                Console.WriteLine("Parametri estratti da Gemma: " + JsonSerializer.Serialize(aiParams));
                Console.WriteLine("Film trovati su TMDB: " + JsonSerializer.Serialize(movies));

                // 3. Ritorna la lista dei film al frontend (Angular/Postman)
                return Ok(movies);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Errore interno del server: {ex.Message}");
            }
        }
    }

