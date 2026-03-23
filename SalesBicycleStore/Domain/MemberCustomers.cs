using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBicycleStore.Domain
{
    public class MemberCustomers : Customer
    {
        public string MemberCode { get; set; } = "";
        public MemberTier Tier { get; set; } = MemberTier.Standard;
        public int Points { get; set; }

        public override decimal GetDiscountPercent(Order order)
        {
            switch (Tier)
            {
                case MemberTier.Standard: return 0.01m;
                case MemberTier.Silver: return 0.02m;
                case MemberTier.Gold: return 0.03m;
                case MemberTier.Platinum: return 0.05m;
                default: return 0m;
            }
        }
    }
}

