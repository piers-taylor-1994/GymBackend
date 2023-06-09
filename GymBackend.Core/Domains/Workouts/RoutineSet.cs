namespace GymBackend.Core.Domains.Workouts
{
    public class RoutineSet
    {
        public Guid Id { get; set; }
        public List<Set> SetList { get; set; }

        public RoutineSet(Guid id, List<Set> setList)
        {
            Id = id;
            SetList = setList;
        }
    }
}
