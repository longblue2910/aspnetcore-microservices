using Contracts.Domains.Interfaces;
using Inventory.Grpc.Entities;
using System.Threading.Tasks;

namespace Inventory.Grpc.Repositories.Interfaces
{
    public interface IInventoryRepository : IMongoDbRepositoryBase<InventoryEntry>
    {
        Task<int> GetStockQuantity(string itemNo);
    }
}
