namespace GymBackend.Core.Domains.Workouts
{
    public class RoutineMuscleArea
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public MuscleArea MuscleArea { get; set; }
    }
}
