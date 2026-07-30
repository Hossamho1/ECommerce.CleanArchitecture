using ECommerce.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Domain.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : BaseEntity;

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}