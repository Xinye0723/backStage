using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace backStage.Models;

public partial class ShowTime
{
    /* 導覽屬性 ↓↓↓ */
    public Movie Movie { get; set; } = null!;
    //public Movie Seat { get; set; } = null!;
    public int ShowTimeId { get; set; }
    [Display(Name = "影廳")]
    public int TheaterNumber { get; set; }
    [Display(Name = "播映日期")]
    public DateOnly ShowDate { get; set; }

    public TimeOnly ShowTime1 { get; set; }
    [Display(Name = "電影編號")]
    public int MovieId { get; set; }
    [Display(Name = "影廳類型")]
    public string ScreenType { get; set; } = null!;
    [Display(Name = "播映起始時間")]
    public DateTime CreatedAt { get; set; }
    [Display(Name = "播映結束時間")]
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<SeatStatus> SeatStatuses { get; set; } = new List<SeatStatus>();
}
