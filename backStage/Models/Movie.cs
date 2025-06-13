using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace backStage.Models;

public partial class Movie
{
    public int MovieId { get; set; }
    
    [Display(Name = "中文片名")]
    public string MovieNameChinese { get; set; } = null!;

    [Display(Name = "英文片名")]
    public string MovieNameEnglish { get; set; } = null!;

    [Display(Name = "類型")]
    public string? Genre { get; set; }

    [Display(Name = "分級")]
    public int MovieRatingId { get; set; }

    public MovieRating? MovieRating { get; set; }

    [Display(Name = "片長")]
    [Range(0, 300, ErrorMessage = "片長請輸入0~300的數字")]
    public int Duration { get; set; }

    [Display(Name = "上映日期")]
    public DateOnly ReleaseDate { get; set; }

    [Display(Name = "下檔日期")]
    public DateOnly EndDate { get; set; }

    [Display(Name = "是否已上映")]
    public bool IsReleased { get; set; }

    [Display(Name = "是否即將上映")]
    public bool IsUpcoming { get; set; }

    [Display(Name = "是否現正熱映")]
    public bool IsNowShowing { get; set; }

    [Display(Name = "是否已下檔")]
    public bool IsEnded { get; set; }

    [Display(Name = "導演")]
    public string? Director { get; set; }

    [Display(Name = "主演")]
    public string? Starring { get; set; }

    [Display(Name = "製作商")]
    public string? Production { get; set; }

    [Display(Name = "發行商")]
    public string? Distributor { get; set; }

    [Display(Name = "國家")]
    public string? Country { get; set; }

    [Display(Name = "劇情")]
    public string? Plot { get; set; }

    [Display(Name = "海報")]
    public string? PosterPicture { get; set; }

    [Display(Name = "預告片")]
    public string? TrailerUrl { get; set; }

    [Display(Name = "點閱次數")]
    public int? ViewCount { get; set; }

    [Display(Name = "票房")]
    [DisplayFormat(DataFormatString ="{0:C}")]
    [Range(0, long.MaxValue, ErrorMessage = "票房不得小於0")]
    public long BoxOffice { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
