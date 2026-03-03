using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using System;
using System.Collections.Generic;

namespace CineBit.Models;

public partial class CinebitDbContext : IdentityDbContext<Utente, IdentityRole<int>, int>
{

    public CinebitDbContext(DbContextOptions<CinebitDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }
    public virtual DbSet<Preferito> Preferiti { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.IdChat).HasName("PRIMARY");

            entity.ToTable("chat");

            entity.HasIndex(e => e.IdUtente, "fk_chat_utente");

            entity.Property(e => e.IdChat).HasColumnName("id_chat");
            entity.Property(e => e.DataCreazione)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("data_creazione");
            entity.Property(e => e.IdUtente).HasColumnName("id_utente");
            entity.Property(e => e.Prompt)
                .HasColumnType("text")
                .HasColumnName("prompt");
            entity.Property(e => e.Response)
                .HasColumnType("text")
                .HasColumnName("response");

            entity.HasOne(d => d.IdUtenteNavigation).WithMany(p => p.Chats)
                .HasForeignKey(d => d.IdUtente)
                .HasConstraintName("fk_chat_utente");
        });

        modelBuilder.Entity<Preferito>(entity =>
        {
            entity.HasKey(e => e.IdPrefe).HasName("PRIMARY");

            entity.ToTable("preferiti");

            entity.HasIndex(e => new { e.IdUtente, e.TmdbId }, "unique_user_movie").IsUnique();

            entity.Property(e => e.IdPrefe).HasColumnName("id_prefe");
            entity.Property(e => e.DataAggiunta)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("data_aggiunta");
            entity.Property(e => e.IdUtente).HasColumnName("id_utente");
            entity.Property(e => e.TitoloCache)
                .HasMaxLength(255)
                .HasColumnName("titolo_cache");
            entity.Property(e => e.TmdbId).HasColumnName("tmdb_id");

            entity.HasOne(d => d.IdUtenteNavigation).WithMany(p => p.Preferitis)
                .HasForeignKey(d => d.IdUtente)
                .HasConstraintName("fk_preferiti_utente");
        });
        
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}