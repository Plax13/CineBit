using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;

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
                new { role = "system", content = @"Sei un assistente AI specializzato nell'interpretare richieste sui film e trasformarle ESCLUSIVAMENTE in parametri di ricerca JSON per TMDB. Non generare MAI testo normale, saluti o spiegazioni.

MAPPA GENERI TMDB COMPLETA:
Azione: 28, Avventura: 12, Animazione: 16, Commedia: 35, Crime: 80, Documentario: 99, Dramma: 18, Famiglia: 10751, Fantasy: 14, Storia: 36, Horror: 27, Musica: 10402, Mistero: 9648, Romantico: 10749, Fantascienza: 878, film TV: 10770, Thriller: 53, Guerra: 10752, Western: 37.

REGOLE DI INTERPRETAZIONE E DEDUZIONE:
1. Se la richiesta è esplicita (es. 'film horror', 'documentario', 'western'), usa il genere corrispondente.
2. Se la richiesta è situazionale o generica, deduci il genere migliore:
   - 'Per bambini', 'con mio figlio piccolo', 'in famiglia' -> Famiglia (10751) o Animazione (16).
   - 'Per ridere', 'serata divertente', 'leggero' -> Commedia (35).
   - 'Per piangere', 'emozionante', 'serio', 'storia vera' -> Dramma (18) o Storia (36).
   - 'Che fa paura', 'notte di halloween', 'spaventoso' -> Horror (27).
   - 'Da risolvere', 'intrigo', 'indagine', 'giallo' -> Mistero (9648) o Thriller (53).
   - 'Per coppie', 'appuntamento', 'storia d'amore' -> Romantico (10749).
3. Se l'utente chiede 'un bel film', 'un capolavoro' o 'da non perdere', imposta vote_average a 7 o 8.
4. actor_query: estrai nomi di attori o registi se presenti.
5. title_query: estrai titoli di film se presenti.
6. Anni: 'Anni 90' -> year_start=1990, year_end=1999; 'Recente' -> year_start=2023.

FORMATO DI RISPOSTA OBBLIGATORIO:
Devi restituire SOLO ED ESCLUSIVAMENTE questo JSON valido:
{ ""title_query"": """", ""actor_query"": """", ""genre_id"": """", ""year_start"": 0, ""year_end"": 0, ""vote_average"": 0 }" },
                new { role = "user", content = "Voglio un film d'azione con Tom Cruise degli anni 2000" },
                new { role = "assistant", content = "{\n  \"title_query\": \"\",\n  \"actor_query\": \"Tom Cruise\",\n  \"genre_id\": \"28\",\n  \"year_start\": 2000,\n  \"year_end\": 2009,\n  \"vote_average\": 0\n}" },
                new { role = "user", content = "Cosa posso guardare stasera con mio figlio piccolo?" },
                new { role = "assistant", content = "{\n  \"title_query\": \"\",\n  \"actor_query\": \"\",\n  \"genre_id\": \"10751\",\n  \"year_start\": 0,\n  \"year_end\": 0,\n  \"vote_average\": 0\n}" },
                new { role = "user", content = "Consigliami una bellissima storia d'amore da guardare con la mia ragazza" },
                new { role = "assistant", content = "{\n  \"title_query\": \"\",\n  \"actor_query\": \"\",\n  \"genre_id\": \"10749\",\n  \"year_start\": 0,\n  \"year_end\": 0,\n  \"vote_average\": 7\n}" },
                new { role = "user", content = input }
            },
            temperature = 0.1
        };
        string jsonGrezzo = "";
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_lmStudioUrl, content);

            // Verifica che LM Studio risponda 200 OK
            response.EnsureSuccessStatusCode();

            var resString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(resString);
            jsonGrezzo = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";

            // --- PULIZIA AVANZATA CON REGEX ---
            // Cerca la prima '{' e l'ultima '}' includendo tutto ciò che sta in mezzo
            var match = Regex.Match(jsonGrezzo, @"\{.*\}", RegexOptions.Singleline);

            if (!match.Success)
                throw new Exception("L'IA non ha restituito un formato JSON valido.");

            string jsonPulito = match.Value;

            // --- DESERIALIZZAZIONE TOLLERANTE ---
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip // Salta eventuali commenti //
            };

            var result = JsonSerializer.Deserialize<AiParameters>(jsonPulito, options);

            // Ritorna il risultato o un oggetto vuoto se null
            return result ?? new AiParameters();
        }
        catch (Exception ex)
        {
            Console.WriteLine("--- Errore Ai ---");
            Console.WriteLine("Messaggio: " + ex.Message);
            Console.WriteLine("Contenuto IA ricevuto: " + jsonGrezzo);

            // Default sicuro: evita il 500 del controller
            return new AiParameters { TitleQuery = "", ActorQuery = "", GenreId = "" };
        }
    }
}