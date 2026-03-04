using System.Text.Json.Serialization;

    public class AiParameters
    {
        [JsonPropertyName("title_query")]
        public string TitleQuery { get; set; }

        [JsonPropertyName("actor_query")]
        public string ActorQuery { get; set; }

        [JsonPropertyName("genre_id")]
        public string GenreId { get; set; }

        [JsonPropertyName("year_start")]
        public int YearStart { get; set; }

        [JsonPropertyName("year_end")]
        public int YearEnd { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }
    }

