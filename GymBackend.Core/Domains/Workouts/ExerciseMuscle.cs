namespace GymBackend.Core.Domains.Workouts
{
    public class ExerciseMuscle
    {
        public int Id { get; set; }
        public Guid ExerciseId { get; set; }
        public MuscleGroup MuscleId { get; set; }
    }
}
