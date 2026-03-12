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
        string movieUrl = $"https://api.themoviedb.org/3/movie/{id}?api_key={_apiKey}&language=it-IT&append_to_response=videos,images,watch/providers";
        var movieResponse = await _httpClient.GetAsync(movieUrl);

        if (!movieResponse.IsSuccessStatusCode)
            return StatusCode((int)movieResponse.StatusCode, "Errore TMDB film");

        var movieJson = await movieResponse.Content.ReadAsStringAsync();
        var movieData = JsonDocument.Parse(movieJson).RootElement;

        // Recupero Credits (Regista e Attori)
        string creditsUrl = $"https://api.themoviedb.org/3/movie/{id}/credits?api_key={_apiKey}&language=it-IT";
        var creditsResponse = await _httpClient.GetAsync(creditsUrl);
        var creditsJson = await creditsResponse.Content.ReadAsStringAsync();
        var creditsData = JsonDocument.Parse(creditsJson).RootElement;

        var directorElement = creditsData.GetProperty("crew")
            .EnumerateArray()
            .FirstOrDefault(x => x.TryGetProperty("job", out var j) && j.GetString() == "Director");

        string regista = directorElement.ValueKind != JsonValueKind.Undefined
            ? directorElement.GetProperty("name").GetString()
            : "Non disponibile";

        var attori = creditsData.GetProperty("cast")
            .EnumerateArray()
            .Take(8)
            .Select(x => new {
                nome = x.TryGetProperty("name", out var n) ? n.GetString() : "Sconosciuto",
                character = x.TryGetProperty("character", out var c) ? c.GetString() : "N/D",
                immagine = x.TryGetProperty("profile_path", out var p) ? p.GetString() : null
            }).ToList();

        // Estrazione Trailer YouTube
        string trailerKey = null;
        if (movieData.TryGetProperty("videos", out var videos))
        {
            var videoList = videos.GetProperty("results").EnumerateArray();
            var trailer = videoList.FirstOrDefault(v =>
                v.TryGetProperty("type", out var t) && t.GetString() == "Trailer" &&
                v.TryGetProperty("site", out var s) && s.GetString() == "YouTube");

            if (trailer.ValueKind != JsonValueKind.Undefined)
                trailerKey = trailer.GetProperty("key").GetString();
        }

        // --- GESTIONE PROVIDERS E WATCH LINK (UNIFICATA) ---
        var providers = new List<object>();
        string watchLink = null;

        if (movieData.TryGetProperty("watch/providers", out var wp) && wp.TryGetProperty("results", out var res))
        {
            if (res.TryGetProperty("IT", out var itProviders))
            {
                // Prendo il link ufficiale TMDB per l'Italia
                if (itProviders.TryGetProperty("link", out var l))
                    watchLink = l.GetString();

                // Estraggo la lista dei servizi streaming (Flatrate)
                if (itProviders.TryGetProperty("flatrate", out var flatrate))
                {
                    providers = flatrate.EnumerateArray().Select(p => new {
                        nome = p.TryGetProperty("provider_name", out var pn) ? pn.GetString() : "N/D",
                        logo = p.TryGetProperty("logo_path", out var lp) ? lp.GetString() : null
                    }).Cast<object>().ToList();
                }
            }
        }

        // Galleria Immagini
        var galleria = new List<string>();
        if (movieData.TryGetProperty("images", out var imgs) && imgs.TryGetProperty("backdrops", out var backdrops))
        {
            galleria = backdrops.EnumerateArray()
                .Take(4)
                .Select(i => i.TryGetProperty("file_path", out var path) ? path.GetString() : null)
                .Where(path => path != null)
                .ToList();
        }

        var risultato = new
        {
            id = id,
            titolo = movieData.TryGetProperty("title", out var title) ? title.GetString() : "Senza Titolo",
            descrizione = movieData.TryGetProperty("overview", out var ov) ? ov.GetString() : "",
            genere = movieData.TryGetProperty("genres", out var gen)
                     ? string.Join(", ", gen.EnumerateArray().Select(g => g.GetProperty("name").GetString()))
                     : "N/D",
            annoUscita = movieData.TryGetProperty("release_date", out var rel) ? rel.GetString()?.Split('-')[0] : "N/A",
            durata = movieData.TryGetProperty("runtime", out var run) && run.ValueKind != JsonValueKind.Null ? run.GetInt32() : 0,
            voto = movieData.TryGetProperty("vote_average", out var vote) ? Math.Round(vote.GetDouble(), 1) : 0,
            regista = regista,
            attori = attori,
            poster_path = movieData.TryGetProperty("poster_path", out var post) ? post.GetString() : null,
            backdrop_path = movieData.TryGetProperty("backdrop_path", out var back) ? back.GetString() : null,
            trailerKey = trailerKey,
            providers = providers,
            watchLink = watchLink,
            galleriaSfondi = galleria
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

    // ==========================
    // FILM SIMILI
    // ==========================
    [HttpGet("{id}/simili")]
    public async Task<IActionResult> GetSimili(int id)
    {
        string url = $"https://api.themoviedb.org/3/movie/{id}/similar?api_key={_apiKey}&language=it-IT&page=1";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, "Errore TMDB simili");

        var json = await response.Content.ReadAsStringAsync();
        var root = JsonDocument.Parse(json).RootElement;

        var simili = root.GetProperty("results")
            .EnumerateArray()
            .Take(12)
            .Select(x => new
            {
                id = x.GetProperty("id").GetInt32(),
                title = x.GetProperty("title").GetString(),
                release_date = x.GetProperty("release_date").GetString(),
                poster_path = x.GetProperty("poster_path").GetString()
            })
            .ToList();

        return Ok(simili);
    }
}