
using System;
using Adhesive.Domain;

namespace Adhesive.Persistence.Imp
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public TRepository CreateRepository<TEntity, TRepository>(IQueryableUnitOfWork unitOfWork)
            where TRepository : IRepository<TEntity>
            where TEntity : Entity
        {
            TRepository repository = (TRepository)Activator.CreateInstance(typeof(TRepository), unitOfWork);
            return repository;
        }
    }
}
