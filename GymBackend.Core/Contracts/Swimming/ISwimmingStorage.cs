using GymBackend.Core.Domains.Workouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymBackend.Core.Contracts.Swimming
{
    public interface ISwimmingStorage
    {
        public Task<Domains.Workouts.Swimming> AddASwimAsync(Guid id, Guid userId, DateTime dateTime, int lengths, int timeSwimming, ReviewEnum review, string explanation);
        public Task<Domains.Workouts.Swimming?> GetTodaysSwimAsync(Guid userId, DateTime today);
        public Task<List<Domains.Workouts.Swimming>> GetRecentSwimsAsync(Guid userId);
        public Task<Domains.Workouts.Swimming> FindASwimAsync(Guid userId, Guid id);
        public Task<Domains.Workouts.Swimming> UpdateASwimAsync(Guid userId, Guid id, int lengths, int timeSwimming, ReviewEnum review, string? explanation);
        public Task DeleteASwimAsync(Guid userId, Guid id);
        public Task<List<Domains.Workouts.Swimming>> GetAllSwimsAsync(Guid userId);
        public Task<int> GetWeeksSwimsAsync(Guid userId, DateTime start, DateTime end);
        public Task<int> GetMonthsSwimsAsync(Guid userId, DateTime yearMonth);
        
    }
    
}
