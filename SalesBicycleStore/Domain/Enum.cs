using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBicycleStore.Domain
{
    public enum OrderStatus { Draft, Confirmed, Paid, Cancelled }
    public enum MemberTier { Standard, Silver, Gold, Platinum }
    // thêm vào các lọai chất liệu
    public enum MaTerial { Aluminum , Steel , Cacbon , Titan }
}

