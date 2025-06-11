using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace backStage.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    [Display(Name = "姓名")]
    public string StaffName { get; set; } = null!;
    [Display(Name = "電話")]
    public string? StaffPhone { get; set; }
    [Display(Name = "Email")]
    public string StaffEmail { get; set; } = null!;
    [Display(Name = "權限等級")]
    public string StaffPermission { get; set; } = null!;
    [Display(Name = "密碼")]
    public string StaffPassword { get; set; } = null!;
}
