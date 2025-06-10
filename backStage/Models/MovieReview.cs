using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class MovieReview
{
    public int MovieReviewId { get; set; }

    public int MovieId { get; set; }

    public int MemberId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime ReviewedAt { get; set; }

    public bool? IsPublic { get; set; }
}
