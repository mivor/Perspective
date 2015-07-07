using System;
using System.Threading.Tasks;

namespace Perspective.Common
{
    public interface IRepository
    {
        Task<TAggregate> GetByIdAsync<TAggregate>(Guid id) where TAggregate : IAggregate;
        Task<TAggregate> GetByIdAsync<TAggregate>(Guid id, int version) where TAggregate : IAggregate;
        Task SaveAsync(IAggregate aggregate);
    }
}