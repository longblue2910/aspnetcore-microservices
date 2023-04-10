using Shared.DTOs.Inventory;

namespace Saga.Orchestrator.HttpRepository.Interfaces
{
    public interface IInventoryHttpRepository
    {
        Task<string> CreateSalesOrder(SalesProductDto model);
        Task<bool> DeletOrderByDocumentNo(string documentNo);
    }
}
