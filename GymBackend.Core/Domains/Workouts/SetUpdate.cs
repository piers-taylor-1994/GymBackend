namespace GymBackend.Core.Domains.Workouts
{
    public class SetUpdate
    {
        public Guid Id { get; set; }
        public MuscleGroup MuscleGroup { get; set; }
        public string Weight { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
    }
}
