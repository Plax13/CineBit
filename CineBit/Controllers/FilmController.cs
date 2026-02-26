using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Linq;

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
            //dev'essere "TMDB:ApiKey"
        }

        // ==========================
        // Endpoint per la "Card" / preview leggera del film
        // ==========================
        // GET api/film/{id}/card
        // Restituisce un oggetto leggero con solo:
        // - Titolo
        // - Anno
        // - Immagine poster
        // - ID del film
        // Utile per popolare la griglia dei risultati senza appesantire il frontend
        [HttpGet("{id}/card")]
        public async Task<IActionResult> GetCardFilm(int id)
        {
            // Costruisce la richiesta all'API TMDB per ottenere i dati base del film
            string url = $"https://api.themoviedb.org/3/movie/{id}?api_key={_apiKey}&language=it-IT";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Errore TMDB");

            // Legge e deserializza il JSON della risposta
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json).RootElement;

            // Costruisce l'oggetto leggero da restituire al frontend
            var card = new
            {
                Id = id,
                Titolo = data.GetProperty("title").GetString(),
                Anno = data.GetProperty("release_date").GetString()?.Substring(0, 4),
                Immagine = "https://image.tmdb.org/t/p/w500" + data.GetProperty("poster_path").GetString()
            };

            return Ok(card);
        }

        // Endpoint per i dettagli completi del film
        // GET api/film/{id}/dettagli
        // Restituisce tutte le informazioni necessarie per la pagina di dettaglio: Titolo, Genere (array), Anno di uscita, Durata, Regist, Attori principali (top 5)
        [HttpGet("{id}/dettagli")]
        public async Task<IActionResult> GetDettagliFilm(int id)
        {
            // Ottengo i dati base del film
            string movieUrl = $"https://api.themoviedb.org/3/movie/{id}?api_key={_apiKey}&language=it-IT";
            var movieResponse = await _httpClient.GetAsync(movieUrl);

            if (!movieResponse.IsSuccessStatusCode)
                return StatusCode((int)movieResponse.StatusCode, "Errore TMDB film");

            var movieJson = await movieResponse.Content.ReadAsStringAsync();
            var movieData = JsonDocument.Parse(movieJson).RootElement;

            //Ottengo i credits per estrarre regista e attori
            string creditsUrl = $"https://api.themoviedb.org/3/movie/{id}/credits?api_key={_apiKey}&language=it-IT";
            var creditsResponse = await _httpClient.GetAsync(creditsUrl);

            if (!creditsResponse.IsSuccessStatusCode)
                return StatusCode((int)creditsResponse.StatusCode, "Errore TMDB credits");

            var creditsJson = await creditsResponse.Content.ReadAsStringAsync();
            var creditsData = JsonDocument.Parse(creditsJson).RootElement;

            // Estrae il regista (job == "Director")
            var crewElement = creditsData.GetProperty("crew")
                .EnumerateArray()
                .FirstOrDefault(x => x.GetProperty("job").GetString() == "Director");

            string regista = crewElement.ValueKind != JsonValueKind.Undefined
                ? crewElement.GetProperty("name").GetString()
                : null;

            // Estraggo i primi 5 attori principali
            var attori = creditsData.GetProperty("cast")
                .EnumerateArray()
                .Take(5)
                .Select(x => x.GetProperty("name").GetString())
                .ToList();

            //  Costruisco oggetto da restituire
            var risultato = new
            {
                Titolo = movieData.GetProperty("title").GetString(),
                Genere = movieData.GetProperty("genres")
                    .EnumerateArray()
                    .Select(g => g.GetProperty("name").GetString()),
                AnnoUscita = movieData.GetProperty("release_date").GetString()?.Substring(0, 4),
                Durata = movieData.GetProperty("runtime").GetInt32(),
                Regista = regista,
                Attori = attori
            };

            return Ok(risultato);
        }
    }
}
