
using Adhesive.Domain;

namespace Adhesive.Persistence
{
    public interface IRepositoryFactory
    {
        TRepository CreateRepository<TEntity, TRepository>(IQueryableUnitOfWork unitOfWork)
            where TRepository : IRepository<TEntity>
            where TEntity : Entity;
    }
}