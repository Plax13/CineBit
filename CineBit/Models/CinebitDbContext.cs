using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace CineBit.Models;

public partial class CinebitDbContext : DbContext
{
    public CinebitDbContext()
    {
    }

    public CinebitDbContext(DbContextOptions<CinebitDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Preferiti> Preferitis { get; set; }

    public virtual DbSet<Utenti> Utentis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

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

        modelBuilder.Entity<Preferiti>(entity =>
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

        modelBuilder.Entity<Utenti>(entity =>
        {
            entity.HasKey(e => e.IdUtente).HasName("PRIMARY");

            entity.ToTable("utenti");

            entity.HasIndex(e => e.Email, "email").IsUnique();



            entity.Property(e => e.IdUtente).HasColumnName("id_utente");
            entity.Property(e => e.Cognome)
                .HasMaxLength(50)
                .HasColumnName("cognome");
            entity.Property(e => e.DataUltimaModifica)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("data_ultima_modifica");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Ruolo)
                .HasMaxLength(20)
                .HasDefaultValueSql("'user'")
                .HasColumnName("ruolo");
            entity.Property(e => e.Stato)
                .HasDefaultValueSql("'1'")
                .HasColumnName("stato");

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
