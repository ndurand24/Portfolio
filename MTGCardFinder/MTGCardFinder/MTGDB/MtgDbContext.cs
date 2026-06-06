using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MTGCardFinder.MTGDB;

public partial class MtgDbContext : DbContext
{
    public MtgDbContext()
    {
    }

    public MtgDbContext(DbContextOptions<MtgDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<CardPrinting> CardPrintings { get; set; }

    public virtual DbSet<Deck> Decks { get; set; }

    public virtual DbSet<DeckCard> DeckCards { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=DeckBuilderdb.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_Cards_name").IsUnique();

            entity.Property(e => e.CardId).HasColumnName("card_id");
            entity.Property(e => e.CreaturePower).HasColumnName("creature_power");
            entity.Property(e => e.CreatureToughness).HasColumnName("creature_toughness");
            entity.Property(e => e.ManaCost).HasColumnName("mana_cost");
            entity.Property(e => e.ManaValue).HasColumnName("mana_value");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.OracleText).HasColumnName("oracle_text");
            entity.Property(e => e.TypeLine).HasColumnName("type_line");
        });

        modelBuilder.Entity<CardPrinting>(entity =>
        {
            entity.HasKey(e => e.PrintingId);

            entity.Property(e => e.PrintingId).HasColumnName("printing_id");
            entity.Property(e => e.Artist).HasColumnName("artist");
            entity.Property(e => e.CardId).HasColumnName("card_id");
            entity.Property(e => e.CollectorNumber).HasColumnName("collector_number");
            entity.Property(e => e.FlavorText).HasColumnName("flavor_text");
            entity.Property(e => e.Rarity).HasColumnName("rarity");
            entity.Property(e => e.SetCode).HasColumnName("set_code");

            entity.HasOne(d => d.Card).WithMany(p => p.CardPrintings).HasForeignKey(d => d.CardId);
        });

        modelBuilder.Entity<Deck>(entity =>
        {
            entity.Property(e => e.DeckId).HasColumnName("deck_id");
            entity.Property(e => e.DeckName).HasColumnName("deck_name");
            entity.Property(e => e.Format).HasColumnName("format");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Decks).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<DeckCard>(entity =>
        {
            entity.HasKey(e => new { e.DeckId, e.CardId, e.IsSideboard });

            entity.Property(e => e.DeckId).HasColumnName("deck_id");
            entity.Property(e => e.CardId).HasColumnName("card_id");
            entity.Property(e => e.IsSideboard).HasColumnName("is_sideboard");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");

            entity.HasOne(d => d.Card).WithMany(p => p.DeckCards).HasForeignKey(d => d.CardId);

            entity.HasOne(d => d.Deck).WithMany(p => p.DeckCards).HasForeignKey(d => d.DeckId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Username).HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
