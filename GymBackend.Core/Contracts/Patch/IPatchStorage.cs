namespace GymBackend.Core.Contracts.Patch
{
    public interface IPatchStorage
    {
        Task<bool> GetUserPatchReadAsync(Guid userId);
        Task<bool> SetUserPatchReadAsync(Guid userId);
    }
}
