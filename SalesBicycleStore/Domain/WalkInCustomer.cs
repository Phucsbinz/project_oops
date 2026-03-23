using SalesBicycleStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesRiceStore.Domain
{
    public class WalkInCustomer : Customer
    {
        public override decimal GetDiscountPercent(Order order) => 0m;
    }
}

