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
    
    // commentiamo la chiamata al modello della chat 
    // al momento non serve preche è stata rimossa la tabella chat dal db 
    // ma nel caso dovessimo inserirla nuovamente serve
    //public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<Preferito> Preferitis { get; set; } = new List<Preferito>();
}


