using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int TheaterNumber { get; set; }

    public string SeatRow { get; set; } = null!;

    public string SeatNumber { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<SeatStatus> SeatStatuses { get; set; } = new List<SeatStatus>();
}
