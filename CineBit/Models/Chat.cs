using System;
using System.Collections.Generic;

namespace CineBit.Models;

public partial class Chat
{
    public int IdChat { get; set; }

    public int IdUtente { get; set; }

    public string Prompt { get; set; } = null!;

    public string Response { get; set; } = null!;

    public DateTime? DataCreazione { get; set; }

    public virtual Utenti IdUtenteNavigation { get; set; } = null!;
}
