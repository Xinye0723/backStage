using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string MovieNameChinese { get; set; } = null!;

    public string MovieNameEnglish { get; set; } = null!;

    public int MovieRatingId { get; set; }

    public int Duration { get; set; }

    public DateOnly ReleaseDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsReleased { get; set; }

    public bool IsUpcoming { get; set; }

    public bool IsNowShowing { get; set; }

    public bool IsEnded { get; set; }

    public string? Director { get; set; }

    public string? Starring { get; set; }

    public string? Production { get; set; }

    public string? Distributor { get; set; }

    public string? Country { get; set; }

    public string? Plot { get; set; }

    public string? PosterPicture { get; set; }

    public string? TrailerUrl { get; set; }

    public int? ViewCount { get; set; }

    public long BoxOffice { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
