using GymBackend.Core.Contracts.Patch;

namespace GymBackend.Service.Patch
{
    public class PatchService : IPatchService
    {
        private readonly IPatchStorage storage;

        public PatchService(IPatchStorage storage)
        {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }
        public async Task<bool> GetUserPatchReadAsync(Guid userId)
        {
            return await storage.GetUserPatchReadAsync(userId);
        }
        public async Task<bool> SetUserPatchReadAsync(Guid userId)
        {
            return await storage.SetUserPatchReadAsync(userId);
        }
    }
}
