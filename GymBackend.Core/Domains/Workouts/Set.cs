namespace GymBackend.Core.Domains.Workouts
{
    public class Set
    {
        public Guid Id { get; set; }
        public MuscleGroup MuscleGroup { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Weight { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
    }
}
