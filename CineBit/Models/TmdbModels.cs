using System.Collections.Generic;
using System.Text.Json.Serialization;

    public class TmdbResponse
    {
        [JsonPropertyName("results")]
        public List<Movie> Results { get; set; }
    }

    public class Movie
    {   
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        // Proprietà aggiunta per facilitare Angular
        public string FullPosterUrl => string.IsNullOrEmpty(PosterPath)
            ? "https://via.placeholder.com/500x750?text=Nessuna+Immagine"
            : $"https://image.tmdb.org/t/p/w500{PosterPath}";

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("overview")]
        public string Overview { get; set; }

        [JsonPropertyName("popularity")]
        public double Popularity { get; set; }
    }
