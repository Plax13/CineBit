namespace CineBit.DTOs;

public class PreferitoDto
{
    public int IdPrefe { get; set; }
    public int TmdbId { get; set; }

    public int IdUtente { get; set; }
    public string? TitoloCache { get; set; }
    public DateTime? DataAggiunta { get; set; }
    public string? PosterPathCache { get; set; }
    public string? GenereCache { get; set; }
}