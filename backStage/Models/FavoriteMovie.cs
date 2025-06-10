using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class FavoriteMovie
{
    public int FavoriteId { get; set; }

    public int MemberId { get; set; }

    public int MovieId { get; set; }

    public DateTime FavoritedAt { get; set; }
}
