﻿using AutoMapper;
using Infrastructure.Mappings;
using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Application.Common.Models;
using Ordering.Domain.Entities;
using Shared.SeedWork;


namespace Ordering.Application.Features.V1.Orders
{
    public class UpdateOrderCommand : CreateOrUpdateCommand, IRequest<ApiResult<OrderDto>>, IMapForm<Order>
    {
        public long Id { get; set; }

        public void SetId(long id)
        {
            Id = id;
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdateOrderCommand, Order>()
                .ForMember(dest => dest.Status, opts => opts.Ignore())
                .IgnoreAllNonExisting();
        }
    }
}
