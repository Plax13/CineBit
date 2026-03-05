using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class FilmController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public FilmController(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = config["TMDB:ApiKey"];
    }

    // ==========================
    // CARD SINGOLA
    // ==========================
    [HttpGet("{id}/card")]
    public async Task<IActionResult> GetCardFilm(int id)
    {
        string url = $"https://api.themoviedb.org/3/movie/{id}?api_key={_apiKey}&language=it-IT";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, "Errore TMDB");

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonDocument.Parse(json).RootElement;

        var card = new
        {
            id = id,
            title = data.GetProperty("title").GetString(),
            release_date = data.GetProperty("release_date").GetString()?.Substring(0, 4),
            poster_path = data.GetProperty("poster_path").GetString()
        };

        return Ok(card);
    }

    // ==========================
    // DETTAGLI FILM
    // ==========================
    [HttpGet("{id}/dettagli")]
    public async Task<IActionResult> GetDettagliFilm(int id)
    {
        string movieUrl = $"https://api.themoviedb.org/3/movie/{id}?api_key={_apiKey}&language=it-IT";
        var movieResponse = await _httpClient.GetAsync(movieUrl);

        if (!movieResponse.IsSuccessStatusCode)
            return StatusCode((int)movieResponse.StatusCode, "Errore TMDB film");

        var movieJson = await movieResponse.Content.ReadAsStringAsync();
        var movieData = JsonDocument.Parse(movieJson).RootElement;

        string creditsUrl = $"https://api.themoviedb.org/3/movie/{id}/credits?api_key={_apiKey}&language=it-IT";
        var creditsResponse = await _httpClient.GetAsync(creditsUrl);

        if (!creditsResponse.IsSuccessStatusCode)
            return StatusCode((int)creditsResponse.StatusCode, "Errore TMDB credits");

        var creditsJson = await creditsResponse.Content.ReadAsStringAsync();
        var creditsData = JsonDocument.Parse(creditsJson).RootElement;

        var crewElement = creditsData.GetProperty("crew")
            .EnumerateArray()
            .FirstOrDefault(x => x.GetProperty("job").GetString() == "Director");

        string regista = crewElement.ValueKind != JsonValueKind.Undefined
            ? crewElement.GetProperty("name").GetString()
            : "";

        var attori = creditsData.GetProperty("cast")
            .EnumerateArray()
            .Take(5)
            .Select(x => x.GetProperty("name").GetString())
            .ToList();

        var risultato = new
        {
            titolo = movieData.GetProperty("title").GetString(),
            genere = movieData.GetProperty("genres")
                .EnumerateArray()
                .Select(g => g.GetProperty("name").GetString()),
            annoUscita = movieData.GetProperty("release_date").GetString()?.Substring(0, 4),
            durata = movieData.GetProperty("runtime").GetInt32(),
            regista = regista,
            attori = attori
        };

        return Ok(risultato);
    }

    // ==========================
    // HOME FILM (per Explore)
    // ==========================
    [HttpGet("home")]
    public async Task<IActionResult> GetHome([FromQuery] int take = 20)
    {
        string url = $"https://api.themoviedb.org/3/movie/popular?api_key={_apiKey}&language=it-IT&page=1";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, "Errore TMDB popular");

        var json = await response.Content.ReadAsStringAsync();
        var root = JsonDocument.Parse(json).RootElement;

        var cards = root.GetProperty("results")
            .EnumerateArray()
            .Take(take)
            .Select(x => new
            {
                id = x.GetProperty("id").GetInt32(),
                title = x.GetProperty("title").GetString(),
                release_date = x.GetProperty("release_date").GetString(),
                poster_path = x.GetProperty("poster_path").GetString()
            })
            .ToList();

        return Ok(cards);
    }
}