using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace backStage.Models;

public partial class MovieGroup
{
    [Display(Name = "群組id")]
    public int GroupId { get; set; }

    [Display(Name = "電影id")]
    public int MovieId { get; set; }

    [Display(Name = "場次ID")]
    public int ShowTimeId { get; set; }

    [Display(Name = "活動名稱")]
    public string? GroupName { get; set; }

    [Display(Name = "主辦人id")]
    public int? LeaderMemberId { get; set; }

    [Display(Name = "最多人數")]
    public int? MaxMembers { get; set; }

    [Display(Name = "活動照片")]
    public string? GroupNote { get; set; }

    [Display(Name = "創建時間")]
    public DateTime? CreateTime { get; set; }

    [Display(Name = "活動介紹")]
    public string? Status { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
