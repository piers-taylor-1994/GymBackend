namespace GymBackend.Core.Contracts.Patch
{
    public interface IPatchStorage
    {
        Task<float> GetUserPatchReadAsync(Guid userId);
        Task<float> SetUserPatchReadAsync(Guid userId, float patch);
    }
}
