using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class Registration
{
    public int RegistrationId { get; set; }

    public int? GroupId { get; set; }

    public int? MemberId { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public int? Members { get; set; }

    public string? Status { get; set; }

    public virtual MovieGroup? Group { get; set; }
}
