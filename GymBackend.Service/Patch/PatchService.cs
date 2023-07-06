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
        public async Task<float> GetUserPatchReadAsync(Guid userId)
        {
            return await storage.GetUserPatchReadAsync(userId);
        }
        public async Task<float> SetUserPatchReadAsync(Guid userId, string patch)
        {
            return await storage.SetUserPatchReadAsync(userId, float.Parse(patch));
        }
    }
}
