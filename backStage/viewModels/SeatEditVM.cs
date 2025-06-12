namespace backStage.viewModels
{
    public class SeatEditVM
    {
        public int TheaterNumber { get; set; }
        public List<SeatPosVM> DeleteSeats { get; set; } = new();
        public List<SeatUpdateVM> UpdateSeats { get; set; } = new();
        public class SeatPosVM
        {
            public int Row { get; set; }
            public int Col { get; set; }
        }

        // 要更新狀態
        public class SeatUpdateVM : SeatPosVM
        {
            public string Status { get; set; }   // occupied / maintenance
        }
    }
}
