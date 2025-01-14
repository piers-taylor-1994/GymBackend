using GymBackend.Core.Contracts.Swimming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymBackend.Storage.Workouts
{
    public class SwimmingStorage : ISwimmingStorage
    {
        public async Task AddASwimAsync(Guid userId, DateTime dateTime, int lengths, int timeSwimming, bool review, string explanation)
        {
            
        }
    }
}
