namespace GymBackend.Core.Domains.Workouts
{
    public class SetOrder
    {
        public Guid Id { get; set; }
        public Guid ExerciseId { get; set; }
        public string Name { get; set; }
        public int Order {  get; set; }
    }
}