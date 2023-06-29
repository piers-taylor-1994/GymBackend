namespace GymBackend.Core.Domains.Workouts
{
    public class SetUpdate
    {
        public string Weight { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int Order { get; set; }

        public SetUpdate(string weight, int sets, int reps, int order)
        {
            Weight = weight;
            Sets = sets;
            Reps = reps;
            Order = order;
        }
    }
}
