using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string MemberName { get; set; } = null!;

    public string MemberPassword { get; set; } = null!;

    public string MemberGender { get; set; } = null!;

    public DateTime MemberBirthDate { get; set; }

    public string MemberEmail { get; set; } = null!;

    public string MemberIntroSelf { get; set; } = null!;

    public string MemberImg { get; set; } = null!;

    public string MemberPermission { get; set; } = null!;
}
