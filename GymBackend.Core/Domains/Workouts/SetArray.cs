namespace GymBackend.Core.Domains.Workouts
{
    public class SetArray
    {
        public int Id { get; set; }
        public Guid SetId { get; set; }
        public float Weight { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int Order { get; set; }
    }
}
