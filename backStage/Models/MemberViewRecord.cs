using System;
using System.Collections.Generic;

namespace backStage.Models;

public partial class MemberViewRecord
{
    public int RecordId { get; set; }

    public string MemberId { get; set; } = null!;

    public int MovieId { get; set; }

    public string MovieNameChinese { get; set; } = null!;

    public string MovieNameEnglish { get; set; } = null!;

    public DateTime WatchDate { get; set; }
}
