namespace GymBackend.Core.Domains.Workouts
{
    public class Exercises
    {
        public Guid Id { get; set; }
        public MuscleGroup MuscleGroup { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
