using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public string SeatRow { get; set; } = null!;

    public string SeatNumber { get; set; } = null!;

    public string TicketType { get; set; } = null!;

    public decimal UnitPrice { get; set; }

    public virtual Order Order { get; set; } = null!;
}
