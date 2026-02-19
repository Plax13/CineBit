using System;
using System.Collections.Generic;

namespace CineBit.Models;

public partial class Utenti
{
    public int IdUtente { get; set; }

    public string Nome { get; set; } = null!;

    public string Cognome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool? Stato { get; set; }

    public string? Ruolo { get; set; }

    public DateTime? DataUltimaModifica { get; set; }

    public int? UtenteUltimaModifica { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<Utenti> InverseUtenteUltimaModificaNavigation { get; set; } = new List<Utenti>();

    public virtual ICollection<Preferiti> Preferitis { get; set; } = new List<Preferiti>();

    public virtual Utenti? UtenteUltimaModificaNavigation { get; set; }
}
