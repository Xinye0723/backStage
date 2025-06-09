using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class Snack
{
    public int SnackId { get; set; }

    public string SnackName { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
