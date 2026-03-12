using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

public class TmdbService
{
    private readonly HttpClient _httpClient;
    private readonly string _tmdbApiKey;

    public TmdbService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        // CORREZIONE: Punta al percorso reale nel tuo appsettings.json
        _tmdbApiKey = config["TMDB:ApiKey"];
    }

    private async Task<string> GetPersonIdAsync(string name)
    {
        try
        {
            string url = $"https://api.themoviedb.org/3/search/person?api_key={_tmdbApiKey}&query={Uri.EscapeDataString(name)}&language=it-IT";
            var response = await _httpClient.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);
            var results = doc.RootElement.GetProperty("results");

            if (results.GetArrayLength() > 0)
            {
                return results[0].GetProperty("id").GetRawText();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore ricerca persona ({name}): {ex.Message}");
        }
        return null;
    }

    public async Task<List<Movie>> SearchMoviesAsync(AiParameters p)
    {
        try
        {
            string url;

            // Logica di costruzione URL (rimasta invariata, è corretta)
            if (!string.IsNullOrWhiteSpace(p.TitleQuery))
            {
                url = $"https://api.themoviedb.org/3/search/movie?api_key={_tmdbApiKey}&query={Uri.EscapeDataString(p.TitleQuery)}&language=it-IT";
            }
            else
            {
                string withCast = "";
                if (!string.IsNullOrWhiteSpace(p.ActorQuery))
                {
                    string id = await GetPersonIdAsync(p.ActorQuery);
                    if (id != null) withCast = $"&with_cast={id}";
                }

                url = $"https://api.themoviedb.org/3/discover/movie?api_key={_tmdbApiKey}&language=it-IT&sort_by=popularity.desc{withCast}";

                if (!string.IsNullOrWhiteSpace(p.GenreId)) url += $"&with_genres={p.GenreId}";
                if (p.VoteAverage > 0) url += $"&vote_average.gte={p.VoteAverage}";
                if (p.YearStart > 0) url += $"&primary_release_date.gte={p.YearStart}-01-01";
                if (p.YearEnd > 0) url += $"&primary_release_date.lte={p.YearEnd}-12-31";
            }

            // Usiamo GetAsync per poter controllare il successo prima di leggere la stringa
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"TMDB ERROR {response.StatusCode}: {errorMsg}");
                return new List<Movie>();
            }

            var res = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<TmdbResponse>(res, options);

            return data?.Results ?? new List<Movie>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore Search: {ex.Message}");
            return new List<Movie>(); // Ritorna lista vuota invece di crashare il server
        }
    }
}