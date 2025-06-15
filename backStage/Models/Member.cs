using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backStage.Models;

public partial class Member
{
    public int MemberId { get; set; }
    [Display(Name = "會員姓名")]
    public string MemberName { get; set; } = null!;
    [Display(Name = "密碼")]
    public string MemberPassword { get; set; } = null!;
    [Display(Name = "性別")]
    public string MemberGender { get; set; } = null!;

    public string MemberGenderDisplay => MemberGender == "M" ? "男" : "女";

    [Display(Name = "生日")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime MemberBirthDate { get; set; }

    public string MemberBirthDateDisplay => MemberBirthDate.ToString("yyyy-MM-dd");
    [Display(Name = "Email")]
    public string MemberEmail { get; set; } = null!;
    [Display(Name = "自我介紹")]

    public string MemberIntroSelf { get; set; } = null!;
    [Display(Name = "會員照片")]
    [NotMapped]
    public IFormFile? MemberImgUpload { get; set; }
    public string MemberImg { get; set; } = null!;
    [Display(Name = "會員等級")]
    public string MemberPermission { get; set; } = null!;
}
