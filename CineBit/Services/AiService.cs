using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class AiService
{
    private readonly HttpClient _httpClient;
    private readonly string _lmStudioUrl;

    public AiService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _lmStudioUrl = config["ApiSettings:LmStudioUrl"];
    }

    public async Task<AiParameters> GetParamsFromGemmaAsync(string input)
    {
        var requestBody = new
        {
            model = "gemma-2-4b-it",
            messages = new[]
            {
        new
        {
            role = "system",
            content = @"Sei un estrattore di parametri JSON.
MAPPA GENERI: Azione:28, Avventura:12, Animazione:16, Commedia:35, Crime:80, Drama:18, Horror:27, Sci-Fi:878, Thriller:53.
REGOLE:
1. actor_query: estrai nomi di attori o registi se presenti.
2. title_query: estrai titoli di film se presenti.
3. Se chiede 'Anni 2000', year_start=2000, year_end=2009.
4. Restituisci ESATTAMENTE E SOLO un JSON valido con questa struttura: 
{ ""title_query"": """", ""actor_query"": """", ""genre_id"": """", ""year_start"": 0, ""year_end"": 0, ""vote_average"": 0 }"
        },
        new
        {
            role = "user",
            content = input
        }
    },
            temperature = 0
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_lmStudioUrl, content);
        var resString = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(resString);
        string jsonGrezzo = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        // Pulizia aggressiva del JSON in caso l'AI aggiunga testo o markdow
        string jsonPulito = Regex.Replace(jsonGrezzo ?? "", @"```json|```", "").Trim();

        // Trova il primo '{' e l'ultimo '}' e prendi solo quello
        int startIndex = jsonPulito.IndexOf('{');
        int endIndex = jsonPulito.LastIndexOf('}');
        if (startIndex >= 0 && endIndex >= 0)
        {
            jsonPulito = jsonPulito[startIndex..(endIndex + 1)]; // range operator più leggibile di Substring
        }

        return JsonSerializer.Deserialize<AiParameters>(jsonPulito);
    }
}
