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
    // CARD SINGOLA (Usata per i suggerimenti o ricerche rapide)
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

        return Ok(new
        {
            id = id,
            title = data.GetProperty("title").GetString(),
            release_date = data.GetProperty("release_date").GetString()?.Substring(0, 4),
            poster_path = data.GetProperty("poster_path").GetString()
        });
    }

    // ==========================
    // DETTAGLI FILM (La pagina principale)
    // ==========================
    [HttpGet("{id}/dettagli")]
    public async Task<IActionResult> GetDettagliFilm(int id)
    {
        string movieUrl = $"https://api.themoviedb.org/3/movie/{id}?api_key={_apiKey}&language=it-IT&append_to_response=videos,images,watch/providers&include_image_language=en,null";
        var movieResponse = await _httpClient.GetAsync(movieUrl);

        if (!movieResponse.IsSuccessStatusCode)
            return StatusCode((int)movieResponse.StatusCode, "Errore TMDB film");

        var movieData = JsonDocument.Parse(await movieResponse.Content.ReadAsStringAsync()).RootElement;

        string creditsUrl = $"https://api.themoviedb.org/3/movie/{id}/credits?api_key={_apiKey}&language=it-IT";
        var creditsData = JsonDocument.Parse(await _httpClient.GetStringAsync(creditsUrl)).RootElement;

        string regista = creditsData.GetProperty("crew").EnumerateArray()
            .FirstOrDefault(x => x.GetProperty("job").GetString() == "Director")
            .GetProperty("name").GetString() ?? "";

        var attori = creditsData.GetProperty("cast").EnumerateArray().Take(10).Select(x => new {
            nome = x.GetProperty("name").GetString(),
            immagine = x.GetProperty("profile_path").GetString(),
            character = x.GetProperty("character").GetString()
        }).ToList();

        var videoKey = "";
        if (movieData.TryGetProperty("videos", out var vEl))
        {
            var video = vEl.GetProperty("results").EnumerateArray()
                .FirstOrDefault(v => v.GetProperty("type").GetString() == "Trailer" && v.GetProperty("site").GetString() == "YouTube");
            if (video.ValueKind != JsonValueKind.Undefined) videoKey = video.GetProperty("key").GetString();
        }

        var sfondi = new List<string>();
        if (movieData.TryGetProperty("images", out var iEl))
        {
            sfondi = iEl.GetProperty("backdrops").EnumerateArray().Take(6)
                .Select(img => img.GetProperty("file_path").GetString()!).ToList();
        }

        var providers = new List<object>();
        if (movieData.TryGetProperty("watch/providers", out var wp))
        {
            var results = wp.GetProperty("results");
            JsonElement countryData;

            if (results.TryGetProperty("IT", out countryData) || results.TryGetProperty("US", out countryData))
            {
                JsonElement list;
                if (countryData.TryGetProperty("flatrate", out list) ||
                    countryData.TryGetProperty("ads", out list) ||
                    countryData.TryGetProperty("rent", out list))
                {
                    providers = list.EnumerateArray().Select(p => new {
                        nome = p.GetProperty("provider_name").GetString(),
                        logo = p.GetProperty("logo_path").GetString()
                    }).Cast<object>().Distinct().ToList();
                }
            }
        }

        return Ok(new
        {
            titolo = movieData.GetProperty("title").GetString(),
            descrizione = movieData.GetProperty("overview").GetString(),
            voto = Math.Round(movieData.GetProperty("vote_average").GetDouble(), 1),
            genere = movieData.GetProperty("genres").EnumerateArray().Select(g => g.GetProperty("name").GetString()),
            annoUscita = movieData.GetProperty("release_date").GetString()?.Split('-')[0],
            durata = movieData.GetProperty("runtime").GetInt32(),
            regista,
            attori,
            poster_path = movieData.GetProperty("poster_path").GetString(),
            backdrop_path = movieData.GetProperty("backdrop_path").GetString(),
            trailerKey = videoKey,
            galleriaSfondi = sfondi,
            providers
        });
    }

    // ==========================
    // HOME FILM (Lista Popular)
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