﻿using AutoMapper;
using Shared.DTOs.Baskets;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;

namespace Saga.Orchestrator
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BasketCheckoutDto, CreateOrderDto>();
            CreateMap<CartItemDto, SalesItemDto>();

        }
    }
}
