using SalesBicycleStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBicycleStore.Pricing
{
    public interface IOrderDiscountPolicy
    {
        decimal ComputeOrderLevelDiscount(Order order);
    }
}

