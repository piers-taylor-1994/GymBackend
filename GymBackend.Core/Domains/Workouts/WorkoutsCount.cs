namespace GymBackend.Core.Domains.Workouts
{
    public class WorkoutsCount
    {
        public int WeekCount { get; set; }
        public int LastWeekCount { get; set; }
        public int MonthCount { get; set; }
        public int LastMonthCount { get; set; }

        public WorkoutsCount(int weekCount, int lastWeekCount, int monthCount, int lastMonthCount)
        {
            WeekCount = weekCount;
            LastWeekCount = lastWeekCount;
            MonthCount = monthCount;
            LastMonthCount = lastMonthCount;
        }
    }
}
