using Inventory.Grpc.Protos;

namespace Basket.API.GrpcService
{
    public class StockItemGrpcService
    {
        private readonly StockProtoService.StockProtoServiceClient _stockProtoService;

        public StockItemGrpcService(StockProtoService.StockProtoServiceClient stockProtoService)
        {
            _stockProtoService = stockProtoService;
        }

        public async Task<StockModel> GetStock(string itemNo)
        {
            try
            {
                var stockItemRequest = new GetStockRequest { ItemNo = itemNo };
                return await _stockProtoService.GetStockAsync(stockItemRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
