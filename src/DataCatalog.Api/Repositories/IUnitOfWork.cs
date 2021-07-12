using System.Threading.Tasks;

namespace DataCatalog.Api.Repositories
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
