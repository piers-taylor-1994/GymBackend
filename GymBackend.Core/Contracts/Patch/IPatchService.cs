namespace GymBackend.Core.Contracts.Patch
{
    public interface IPatchService
    {
        Task<bool> GetUserPatchReadAsync(Guid userId);
        Task<bool> SetUserPatchReadAsync(Guid userId);
    }
}
