namespace GymBackend.Core.Domains.Workouts
{
    public class NewSet
    {
        public Guid ExerciseId { get; set; }
        public float Weight { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int Order { get; set; }
    }
}
