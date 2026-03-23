using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBicycleStore.Pricing
{
    public interface ITaxCalculator
    {
        decimal ComputeTax(decimal taxableAmount);
    }
}
