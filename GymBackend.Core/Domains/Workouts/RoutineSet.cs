namespace GymBackend.Core.Domains.Workouts
{
    public class RoutineSet
    {
        public Guid Id { get; set; }
        public List<ExerciseSets> ExerciseSets { get; set; }

        public RoutineSet(Guid id, List<ExerciseSets> exerciseSets)
        {
            Id = id;
            ExerciseSets = exerciseSets;
        }
    }
}
