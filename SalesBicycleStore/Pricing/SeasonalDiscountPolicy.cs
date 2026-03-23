using SalesBicycleStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SalesBicycleStore.Pricing
{
    public class SeasonalDiscountPolicy : IOrderDiscountPolicy
    {
        public decimal ComputeOrderLevelDiscount(Order order)
        {
            int month = order.OrderDate.Month;

            if (month >= 2 && month <= 4)
                return 0.05m; // Xuân
            else if (month >= 5 && month <= 7)
                return 0.10m; // Hạ
            else if (month >= 8 && month <= 10)
                return 0.07m; // Thu
            else
                return 0.15m; // Đông
        }
    }
}
