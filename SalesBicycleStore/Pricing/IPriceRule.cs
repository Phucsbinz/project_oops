using SalesBicycleStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBicycleStore.Pricing
{
    public interface IPriceRule
    {
        decimal ComputeLineAmount(OrderLine line);
    }
}

