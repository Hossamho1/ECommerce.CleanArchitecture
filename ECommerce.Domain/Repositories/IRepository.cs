using ECommerce.Domain.Entities;
using ECommerce.Domain.Specifications;

namespace ECommerce.Domain.Repositories;

public interface IRepository<T>: IReadRepository<T> where T : BaseEntity
{
    void Add(T entity);

    void Update(T entity);

    void Delete(T entity);
}