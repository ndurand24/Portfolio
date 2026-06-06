using System;
using System.Collections.Generic;

namespace MTGCardFinder.MTGDB;

public partial class DeckCard
{
    public int DeckId { get; set; }

    public int CardId { get; set; }

    public int Quantity { get; set; }

    public int IsSideboard { get; set; }

    public virtual Card Card { get; set; } = null!;

    public virtual Deck Deck { get; set; } = null!;
}
