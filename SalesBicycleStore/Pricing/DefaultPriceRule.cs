using SalesBicycleStore.Domain;
using SalesBicycleStore.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesRiceStore.Pricing
{
    public class DefaultPriceRule : IPriceRule
    {
        public decimal ComputeLineAmount(OrderLine l) => l.LineAmount();
    }
}
