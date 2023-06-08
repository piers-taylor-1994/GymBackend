namespace GymBackend.Core.Domains.Workouts
{
    public class Routine
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
    }
}
