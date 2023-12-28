using GymBackend.Core.Domains.Workouts;

namespace GymBackend.API.Models
{
    public class RecentWorkout
    {
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public MuscleArea MuscleArea { get; set; }
    }
}
