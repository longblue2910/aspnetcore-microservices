using AutoMapper;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Inventory.Product.API.Entities;
using Inventory.Product.API.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Configurations;
using Shared.DTOs.Inventory;
using Shared.SeedWork;

namespace Inventory.Product.API.Services
{
    public class InventoryService : MongoDbRepository<InventoryEntry>, IInventoryService
    {
        private readonly IMapper _mapper;

        public InventoryService(IMongoClient client, MongoDbSettings settings, IMapper mapper) : base(client, settings)
        {
            _mapper = mapper;
        }    
        public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo)
        {
            var entities = await FindAll().Find(x => x.ItemNo.Equals(itemNo))
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);
            return result;
        }

        public async Task<PagedList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query)
        {
            var filterSearchTerm = Builders<InventoryEntry>.Filter.Empty;
            var filterItemNo = Builders<InventoryEntry>.Filter.Eq(x => x.ItemNo, query.ItemNo());
            if (!string.IsNullOrEmpty(query.SearchTerm))
                filterSearchTerm = Builders<InventoryEntry>.Filter.Eq(x => x.DocumentNo, query.SearchTerm);

            var andFilter = filterItemNo & filterSearchTerm;
            var pageList = await Collection.PaginatedListAsync(andFilter,
                pageIndex: query.PageIndex, pageSize: query.PageSize);

            var items = _mapper.Map<IEnumerable<InventoryEntryDto>>(pageList);
            var result = new PagedList<InventoryEntryDto>(items, pageList.GetMetaData().TotalItems,
                pageIndex: query.PageIndex, pageSize: query.PageSize);
            return result;
        }

        public async Task<InventoryEntryDto> GetByIdAsync(string id)
        {
            FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Eq(x => x.Id, id);
            var entity = await FindAll().Find(filter).FirstOrDefaultAsync();
            var result = _mapper.Map<InventoryEntryDto>(entity);

            return result;
        }

        public async Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model)
        {
            var entity = new InventoryEntry(ObjectId.GenerateNewId().ToString())
            {
                DocumentType = Shared.Enums.Inventory.EDocumentType.Purchase,
                DocumentNo = Guid.NewGuid().ToString(),
                ExternalDocumentNo = Guid.NewGuid().ToString(),
                ItemNo = model.ItemNo,
                Quantity = model.Quantity,
            };

            await CreateAsync(entity);
            var result = _mapper.Map<InventoryEntryDto>(entity);
            return result;
        }   
    }
}
