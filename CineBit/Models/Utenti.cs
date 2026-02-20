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



    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

 

    public virtual ICollection<Preferiti> Preferitis { get; set; } = new List<Preferiti>();


}
