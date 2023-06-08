namespace GymBackend.Core.Domains.Workouts
{
    public class RoutineView
    {
        public Guid Id { get; set; }
        public List<Set> SetList { get; set; }

        public RoutineView(Guid id, List<Set> setList)
        {
            Id = id;
            SetList = setList;
        }
    }
}
