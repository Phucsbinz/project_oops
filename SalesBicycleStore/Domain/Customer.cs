using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBicycleStore.Domain
{
    public abstract class Customer
    {
        public string CustomerId { get; set; } = System.Guid.NewGuid().ToString();
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public bool DirectOrIndirect {  get; set; }=false;
        public abstract decimal GetDiscountPercent(Order order);
    }
}

