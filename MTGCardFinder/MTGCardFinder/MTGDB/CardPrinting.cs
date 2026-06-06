using System;
using System.Collections.Generic;

namespace MTGCardFinder.MTGDB;

public partial class CardPrinting
{
    public int PrintingId { get; set; }

    public int CardId { get; set; }

    public string SetCode { get; set; } = null!;

    public string CollectorNumber { get; set; } = null!;

    public string Rarity { get; set; } = null!;

    public string? Artist { get; set; }

    public string? FlavorText { get; set; }

    public virtual Card Card { get; set; } = null!;
}
