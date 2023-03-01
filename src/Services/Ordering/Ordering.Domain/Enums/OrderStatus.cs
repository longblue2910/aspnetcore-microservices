﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Enums
{
    public enum OrderStatus
    {
        New = 1,
        Pending,
        Paid,
        Shipping,
        Fulfilled
    }
}
