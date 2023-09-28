namespace GymBackend.Core.Domains.Workouts
{
    public class ExerciseSets
    {
        public Guid Id { get; set; }
        public Guid ExerciseId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public List<SetSet> ExerciseArray { get; set; }

        public ExerciseSets (Guid id, Guid exerciseId, string name, int order, List<SetSet> exerciseArray)
        {
            Id = id;
            ExerciseId = exerciseId;
            Name = name;
            Order = order;
            ExerciseArray = exerciseArray;
        }
    }
}
