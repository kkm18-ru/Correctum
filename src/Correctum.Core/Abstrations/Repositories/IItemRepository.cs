using Correctum.Models;

namespace Correctum.Abstrations.Repositories
{
    public interface IItemRepository
    {
        Task CreateAsync(Item item);
        Task<Item> GetByIdAsync(int id);
        IAsyncEnumerable<Item> GetAllAsync();
        Task UpdateAsync(Item item);
        Task MultiUpdateAsync(params ICollection<Item> items);
    }
}
