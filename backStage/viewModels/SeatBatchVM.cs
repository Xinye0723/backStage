using System.ComponentModel.DataAnnotations;

namespace backStage.viewModels
{
    public class SeatBatchVM
    {
        [Required]
        public int TheaterNumber { get; set; }

        // 預設 15 排 25 座，不想讓使用者改可移除
        [Range(1, 26)]
        public int Rows { get; set; } = 13;

        [Range(1, 100)]
        public int SeatsPerRow { get; set; } = 25;
    }
}
