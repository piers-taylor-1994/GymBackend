using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymBackend.Core.Contracts.Swimming
{
    public interface ISwimmingStorage
    {
        public Task AddASwimAsync(Guid id, Guid userId, DateTime dateTime, int lengths, int timeSwimming, bool review, string explanation);
    }
}
