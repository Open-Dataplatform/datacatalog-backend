namespace DataCatalog.Api.Services
{
    public interface IPermissionUtils
    {
        bool IsAdminOrDataSteward { get; }
        bool FilterUnpublishedDatasets { get; }
        bool IsAdmin { get; }
    }
}