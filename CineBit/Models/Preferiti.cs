using System;
using System.Collections.Generic;

namespace CineBit.Models;

public partial class Preferiti
{
    public int IdPrefe { get; set; }

    public int IdUtente { get; set; }

    public int TmdbId { get; set; }

    public string? TitoloCache { get; set; }

    public DateTime? DataAggiunta { get; set; }

    public virtual Utenti IdUtenteNavigation { get; set; } = null!;
}
