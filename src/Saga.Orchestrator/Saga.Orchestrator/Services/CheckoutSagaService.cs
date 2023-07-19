using AutoMapper;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOs.Baskets;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;
using ILogger = Serilog.ILogger;

namespace Saga.Orchestrator.Services
{
    public class CheckoutSagaService : ICheckoutSagaService
    {
        private readonly IOrderHttpRepository _orderHttpRep;
        private readonly IBasketHttpRepository _basketHttpRe;
        private readonly IInventoryHttpRepository _inventoryHttpRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CheckoutSagaService(IOrderHttpRepository orderHttpRep, IBasketHttpRepository basketHttpRe, 
            IInventoryHttpRepository inventoryHttpRepository, IMapper mapper, ILogger logger)
        {
            _orderHttpRep = orderHttpRep;
            _basketHttpRe = basketHttpRe;
            _inventoryHttpRepository = inventoryHttpRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<bool> CheckoutOrderAsync(string username, BasketCheckoutDto basketCheckout)
        {
            //Get cart from BasketHttpRepository
            _logger.Information($"Start: Get cart {username}");

            var cart = await _basketHttpRe.GetBasket(username);
            if (cart == null) return false;

            _logger.Information($"End: Get cart {username} success");

            //Create Order from OrderHttpRepository
            _logger.Information($"Start: Create Order");

            var order = _mapper.Map<CreateOrderDto>(basketCheckout);
            order.TotalPrice = cart.TotalPrice;

            //Get order by Order Id
            var orderId = await _orderHttpRep.CreateOrder(order);
            if (orderId < 0) return false;
            var addedOrder = await _orderHttpRep.GetOrder(orderId);

            _logger.Information($"End: Create Order success, Order Id: {orderId} - Document No - {addedOrder.DocumentNo}");

            var inventoryDocumentNos = new List<string>();
            bool result = true;
            try
            {
                //Sale Items from InventoryHttpRepository
                foreach (var item in cart.Items)
                {
                    _logger.Information($"Start: Sale Item No: {item.ItemNo} - Quantity: {item.Quantity}");

                    var saleOrder = new SalesProductDto(addedOrder.DocumentNo, item.Quantity);
                    saleOrder.SetItemNo(saleOrder.ItemNo);
                    var documentNo = await _inventoryHttpRepository.CreateSalesOrder(saleOrder);
                    inventoryDocumentNos.Add(documentNo);

                    _logger.Information($"End: Sale Item No: {item.ItemNo} " +
                        $"- Quantity: {item.Quantity} - Document No: {documentNo}");
                }

                //Delete Basket
                result = await _basketHttpRe.DeleteBasket(username);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                await RollbackCheckoutOrder(username, addedOrder.Id, inventoryDocumentNos);
                result = false;
            }

            //Rollback checkout order

            return result;
        }

        private async Task RollbackCheckoutOrder(string userName, long orderId, List<string> inventoryDocumentNos)
        {
            _logger.Information($"Start: RollbackCheckoutOrder for username: {userName} " +
                                $"order id: {orderId}, " +
                                $"inventory document nos: {String.Join(",", inventoryDocumentNos)}");

            var deletedDocumentNos = new List<string>();

            _logger.Information($"Start: Deleted Order Id: {orderId}");
            await _orderHttpRep.DeleteOrder(orderId);

            _logger.Information($"Start: Deleted Order Id: {orderId}");


            foreach (var documentNo in inventoryDocumentNos)
            {
                await _inventoryHttpRepository.DeletOrderByDocumentNo(documentNo);
                deletedDocumentNos.Add(documentNo);
            }
            _logger.Information($"End: Deleted Inventory Document Nos: ");
        }
    }
}
