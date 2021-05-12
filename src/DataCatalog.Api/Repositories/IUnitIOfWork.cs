using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public interface IUnitIOfWork
    {
        Task CompleteAsync();
    }
}
