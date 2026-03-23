using SalesBicycleStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBicycleStore.Pricing
{
    public class NoOrderDiscount : IOrderDiscountPolicy
    {
        public decimal ComputeOrderLevelDiscount(Order order) => 0m;
    }
}

