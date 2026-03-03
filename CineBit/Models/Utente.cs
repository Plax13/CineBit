using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace CineBit.Models;

public partial class Utente : IdentityUser<int>
{
    public int IdUtente { get; set; }

    public string Nome { get; set; } = null!;

    public string Cognome { get; set; } = null!;

    public bool? Stato { get; set; }

    public Role Ruolo { get; set; } = Role.Utente;

    public DateTime? DataUltimaModifica { get; set; }


    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<Preferito> Preferitis { get; set; } = new List<Preferito>();


}