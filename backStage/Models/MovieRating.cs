using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace backStage.Models;

public partial class MovieRating
{
    public int MovieRatingId { get; set; }

    public string RatingCode { get; set; } = null!;

    public string Description { get; set; } = null!;

    public ICollection<Movie>? Movies { get; set; }

    [NotMapped]
    public string FullName => $"{Description}（{RatingCode}）";
}
