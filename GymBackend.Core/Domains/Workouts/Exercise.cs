namespace GymBackend.Core.Domains.Workouts
{
    public class Exercise
    {
        public Guid ExerciseId { get; set; }
        public MuscleGroup MuscleGroup { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
