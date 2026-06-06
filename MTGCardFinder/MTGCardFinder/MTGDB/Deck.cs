using System;
using System.Collections.Generic;

namespace MTGCardFinder.MTGDB;

public partial class Deck
{
    public int DeckId { get; set; }

    public string? DeckName { get; set; }

    public string Format { get; set; } = null!;

    public int UserId { get; set; }

    public virtual ICollection<DeckCard> DeckCards { get; set; } = new List<DeckCard>();

    public virtual User User { get; set; } = null!;
}
