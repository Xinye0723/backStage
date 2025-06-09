using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public string Status { get; set; } = null!;

    public int MovieId { get; set; }

    public int ShowTimeId { get; set; }

    public int TheaterNumber { get; set; }

    public int OrderNumbers { get; set; }

    public decimal OrderPrice { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
