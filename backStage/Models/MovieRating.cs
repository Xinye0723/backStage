using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class MovieRating
{
    public int MovieRatingId { get; set; }

    public string RatingCode { get; set; } = null!;

    public string Description { get; set; } = null!;
}
