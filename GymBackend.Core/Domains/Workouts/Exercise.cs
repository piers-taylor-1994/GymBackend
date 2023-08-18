namespace GymBackend.Core.Domains.Workouts
{
    public class Exercise
    {
        public Guid ExerciseId { get; set; }
        public MuscleArea MuscleArea { get; set; }
        public string Name { get; set; }
    }
}