using Infrastructure.Common;
using Inventory.Grpc.Entities;
using Inventory.Grpc.Repositories.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Shared.Configurations;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Grpc.Repositories
{
    public class InventoryRepository : MongoDbRepository<InventoryEntry>, IInventoryRepository
    {
        public InventoryRepository(IMongoClient client, MongoDbSettings settings) : base(client, settings)
        {
        }

        public Task<int> GetStockQuantity(string itemNo)
            => Task.FromResult(Collection.AsQueryable()
            .Where(x => x.ItemNo.Equals(itemNo))
            .Sum(x => x.Quantity));
    }
}
