namespace GymBackend.Core.Domains.Workouts
{
    public class RoutineTemplate
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }
}
