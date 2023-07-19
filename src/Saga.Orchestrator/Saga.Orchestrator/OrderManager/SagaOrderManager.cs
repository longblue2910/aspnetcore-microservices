using AutoMapper;
using Contracts.Saga.OrderManager;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Baskets;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;

namespace Saga.Orchestrator.OrderManager
{
    public class SagaOrderManager : ISagaOrderManager<BasketCheckoutDto, OrderResponse>
    {
        private readonly IOrderHttpRepository _orderHttpRep;
        private readonly IBasketHttpRepository _basketHttpRe;
        private readonly IInventoryHttpRepository _inventoryHttpRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public SagaOrderManager(IOrderHttpRepository orderHttpRep, IBasketHttpRepository basketHttpRe,
            IInventoryHttpRepository inventoryHttpRepository, IMapper mapper, ILogger logger)
        {
            _orderHttpRep = orderHttpRep;
            _basketHttpRe = basketHttpRe;
            _inventoryHttpRepository = inventoryHttpRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public OrderResponse CreateOrder(BasketCheckoutDto input)
        {
            var orderStateMachine = 
                new Stateless.StateMachine<EOrderTransactionState, EOrderAction>(EOrderTransactionState.NotStarted);

            long orderId = -1;
            CartDto cart = null;
            OrderDto addOrder= null;
            string inventoryDocumentNo = string.Empty;

            orderStateMachine.Configure(EOrderTransactionState.NotStarted)
                .PermitDynamic(EOrderAction.GetBasket, () =>
                {
                    cart = _basketHttpRe.GetBasket(input.UserName).Result;
                    return cart != null ? EOrderTransactionState.BasketGot : EOrderTransactionState.BasketGetFailed;
                });

            orderStateMachine.Configure(EOrderTransactionState.BasketGot)
                .PermitDynamic(EOrderAction.CreateOrder, () =>
                {
                    var order = _mapper.Map<CreateOrderDto>(input);
                    order.TotalPrice = cart.TotalPrice;
                    orderId = _orderHttpRep.CreateOrder(order).Result;
                    return orderId > 0 ? EOrderTransactionState.OrderCreated : EOrderTransactionState.OrderCreatedFailed;
                })
                .OnEntry(() => orderStateMachine.Fire(EOrderAction.CreateOrder));

            orderStateMachine.Configure(EOrderTransactionState.NotStarted)
                .PermitDynamic(EOrderAction.GetBasket, () =>
                {
                    addOrder = _orderHttpRep.GetOrder(orderId).Result;
                    return addOrder != null ? EOrderTransactionState.OrderGot : EOrderTransactionState.OrderGetFailed;
                }).OnEntry(() => orderStateMachine.Fire(EOrderAction.GetOrder));

            orderStateMachine.Configure(EOrderTransactionState.OrderGot)
                .PermitDynamic(EOrderAction.UpdateInventory, () =>
                {
                    var salesOrder = new SalesOrderDto
                    {
                        OrderNo = addOrder.DocumentNo,
                        SalesItems = _mapper.Map<List<SalesItemDto>>(cart.Items)
                    };
                    inventoryDocumentNo = 
                        _inventoryHttpRepository.CreateOrderSale(addOrder.DocumentNo, salesOrder).Result;

                    return inventoryDocumentNo != null
                     ? EOrderTransactionState.InventoryUpdated
                     : EOrderTransactionState.InventoryUpdateFailed;

                }).OnEntry(() => orderStateMachine.Fire(EOrderAction.UpdateInventory));

            orderStateMachine.Configure(EOrderTransactionState.InventoryUpdated)
                .PermitDynamic(EOrderAction.DeleteBasket, () =>
                {
                    var result = _basketHttpRe.DeleteBasket(input.UserName).Result;
                    return result ? EOrderTransactionState.BasketDeleted : EOrderTransactionState.InventoryUpdateFailed;
                }).OnEntry(() => orderStateMachine.Fire(EOrderAction.DeleteBasket));

            orderStateMachine.Configure(EOrderTransactionState.InventoryUpdateFailed)
                .PermitDynamic(EOrderAction.DeleteInventory, () =>
                {
                    RollbackOrder(input.UserName, inventoryDocumentNo, orderId);
                    return EOrderTransactionState.InventoryRollback;
                }).OnEntry(() => orderStateMachine.Fire(EOrderAction.DeleteInventory));

            orderStateMachine.Fire(EOrderAction.GetBasket);

            return new OrderResponse(orderStateMachine.State == EOrderTransactionState.BasketDeleted);
        }

        public OrderResponse RollbackOrder(string username, string documentNo, long orderId)
        {
            var orderStateMachine =
                new Stateless.StateMachine<EOrderTransactionState, EOrderAction>(EOrderTransactionState.RollbackInventory);

            orderStateMachine.Configure(EOrderTransactionState.RollbackInventory)
                .PermitDynamic(EOrderAction.DeleteInventory, () =>
                {
                    _inventoryHttpRepository.DeletOrderByDocumentNo(documentNo);
                    return EOrderTransactionState.InventoryRollback;
                });

            orderStateMachine.Configure(EOrderTransactionState.InventoryRollback)
                .PermitDynamic(EOrderAction.DeleteOrder, () =>
                {
                    var result = _orderHttpRep.DeleteOrder(orderId).Result;
                    return result
                    ? EOrderTransactionState.InventoryRollback
                    : EOrderTransactionState.InventoryRollbackFailed;
                }).OnEntry(() => orderStateMachine.Fire(EOrderAction.DeleteOrder));

            orderStateMachine.Fire(EOrderAction.DeleteInventory);

            return new OrderResponse(orderStateMachine.State == EOrderTransactionState.InventoryRollback);
        }
    }
}
