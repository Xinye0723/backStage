using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class SeatStatus
{
    public int SeatStatusId { get; set; }

    public int ShowTimeId { get; set; }

    public int SeatId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public virtual Seat Seat { get; set; } = null!;

    public virtual ShowTime ShowTime { get; set; } = null!;
}
