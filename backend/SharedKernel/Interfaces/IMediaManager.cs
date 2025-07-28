namespace SharedKernel.Interfaces
{
    public interface IMediaManager
    {
        Task<IEnumerable<string>> GetAllEntityMediaUrls(Guid entityId);
        Task<Dictionary<Guid,string>> GetAllMainMediaUrls(IEnumerable<Guid> entityIds);
        Task<string> GetMainMediaUrl(Guid entityId);
    }
}
