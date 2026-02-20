using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Web;

namespace CineBit.Controllers
{
    // Questo attributo imposto il routing dell'API
    //tutte le chiamate partiranno da api/film
    [Route("api/[controller]")]
    [ApiController]
    public class FilmController : ControllerBase
    {
        
        // HttpClient per fare richieste HTTp verso l'API TMDB
        private readonly HttpClient _httpClient;

        //construttore: injection IHttpClientFactory tramite Dependency Injection
        private readonly string _apiKey;

        public FilmController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient();

            // Legge la chiave API dal JSON: appsettings.json deve avere
            // "TMDB": { "ApiKey": "LA_TUA_CHIAVE" }
            _apiKey = config["TMDB:ApiKey"];
            // ATTENZIONE: nel tuo codice attuale usi "ApiKey" -> dev'essere "TMDB:ApiKey"
        }

        /*Ricerca Film tramit parametri passati nel body della richiesta,
         Endpoint POST: /api/film/ricerca
        Oggetto name = "dto" oggetto contenente Titolo Genere Anno
        Json con risultati della ricerca trmite TMDB*/
        [HttpPost("ricerca")]
        public async Task<IActionResult> Ricerca([FromBody] FilmRicercaDto dto)
        {
            // Collection di query string vuota
            var queryParams = HttpUtility.ParseQueryString(string.Empty);

            //Se titolo, genere, anno non sono null, lo aggiungiamo
            if (!string.IsNullOrEmpty(dto.Titolo))
                queryParams["query"] = dto.Titolo;
            if (!string.IsNullOrEmpty(dto.Genere))
                queryParams["with_genres"] = dto.Genere;
            if (dto.Anno.HasValue)
                queryParams["primary_release_year"] = dto.Anno.Value.ToString();

            //es : query=Inception&with_genres=28&primary_release_year=2010


            // Utl completo API TMDB
            string url = $"https://api.themoviedb.org/3/search/movie?api_key={ApiKey}&{queryParams}";
            
            // Chiamata http get su TMDB
            var response = await _httpClient.GetAsync(url);
            
            //Se la chiamata non funge, ritorno l'errore con codice Http
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Errore API TMDB");

            // leggo il contenuto del json
            var json = await response.Content.ReadAsStringAsync();

            //deserializzo il json in modo generico
            var results = JsonSerializer.Deserialize<JsonElement>(json);

            //ritorno json
            return Ok(results);
        }
    }
}
