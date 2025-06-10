using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public string StaffName { get; set; } = null!;

    public int StaffPhone { get; set; }

    public string StaffEmail { get; set; } = null!;

    public string StaffPermission { get; set; } = null!;

    public string StaffPassword { get; set; } = null!;
}
