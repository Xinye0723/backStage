using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class MovieGroup
{
    public int GroupId { get; set; }

    public int MovieId { get; set; }

    public int ShowTimeId { get; set; }

    public string? GroupName { get; set; }

    public int? LeaderMemberId { get; set; }

    public int? MaxMembers { get; set; }

    public string? GroupNote { get; set; }

    public DateTime? CreateTime { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
