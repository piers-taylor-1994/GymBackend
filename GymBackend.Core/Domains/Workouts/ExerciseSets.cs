namespace GymBackend.Core.Domains.Workouts
{
    public class ExerciseSets
    {
        public Guid Id { get; set; }
        public Guid ExerciseId { get; set; }
        public string Name { get; set; }
        public ExerciseType Type { get; set; }
        public int Order { get; set; }
        public List<SetArray> ExerciseArray { get; set; }

        public ExerciseSets (Guid id, Guid exerciseId, string name, ExerciseType type, int order, List<SetArray> exerciseArray)
        {
            Id = id;
            ExerciseId = exerciseId;
            Name = name;
            Type = type;
            Order = order;
            ExerciseArray = exerciseArray;
        }
    }
}
