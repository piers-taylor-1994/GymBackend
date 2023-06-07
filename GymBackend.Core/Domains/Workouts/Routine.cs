namespace GymBackend.Core.Domains.Workouts
{
    public class Routine
    {
        public List<Set> SetList { get; set; }

        public Routine(List<Set> setList)
        {
            SetList = setList;
        }
    }
}
