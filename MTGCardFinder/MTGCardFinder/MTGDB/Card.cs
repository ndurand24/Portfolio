using System;
using System.Collections.Generic;

namespace MTGCardFinder.MTGDB;

public partial class Card
{
    public int CardId { get; set; }

    public string Name { get; set; } = null!;

    public string? ManaCost { get; set; }

    public int? ManaValue { get; set; }

    public string TypeLine { get; set; } = null!;

    public string? OracleText { get; set; }

    public string? CreaturePower { get; set; }

    public string? CreatureToughness { get; set; }

    public virtual ICollection<CardPrinting> CardPrintings { get; set; } = new List<CardPrinting>();

    public virtual ICollection<DeckCard> DeckCards { get; set; } = new List<DeckCard>();
}
