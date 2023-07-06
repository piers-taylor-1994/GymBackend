namespace GymBackend.Core.Contracts.Patch
{
    public interface IPatchService
    {
        Task<float> GetUserPatchReadAsync(Guid userId);
        Task<float> SetUserPatchReadAsync(Guid userId, string patch);
    }
}
