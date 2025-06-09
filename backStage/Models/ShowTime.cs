using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class ShowTime
{
    public int ShowTimeId { get; set; }

    public int TheaterNumber { get; set; }

    public DateOnly ShowDate { get; set; }

    public TimeOnly ShowTime1 { get; set; }

    public int MovieId { get; set; }

    public string ScreenType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<SeatStatus> SeatStatuses { get; set; } = new List<SeatStatus>();
}
