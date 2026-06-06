using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesBicycleStore.Pricing;

namespace SalesBicycleStore.Domain
{
    public class Order      
    {
        public decimal FeeShip { get; set; } = 300000m;
        public string OrderNo { get; private set; } = DateTime.Now.Ticks.ToString();
        public DateTime OrderDate { get; private set; } = DateTime.Now;
        public Customer Customer { get; private set; }
        public OrderStatus Status { get; private set; } = OrderStatus.Draft;
        public List<OrderLine> Lines { get; private set; } = new List<OrderLine>();
       // public Customer DerectOrInderect {  get; private set; }
        public decimal Subtotal { get; private set; }
        public decimal CustomerDiscount { get; private set; }
        public decimal DiscountOnOrder { get; private set; }
        public decimal VATPercent { get; set; } = 0.08m;
        public decimal VAT { get; private set; }
        public decimal Total { get; private set; }
        private readonly IPriceRule _priceRule;
        private readonly IOrderDiscountPolicy _orderDiscount;
        private readonly IOrderDiscountPolicy _orderSeasonalDiscount;
        private readonly ITaxCalculator _tax;

        public event EventHandler<OrderStatusChangedEventArgs> OrderStatusChanged;
        public Order(Customer customer, IPriceRule priceRule, IOrderDiscountPolicy orderDiscount, IOrderDiscountPolicy orderSeasonalDiscount, ITaxCalculator tax)
        {
            if (customer == null) throw new ArgumentNullException(nameof(customer));
            Customer = customer; _priceRule = priceRule; _orderDiscount = orderDiscount; _tax = tax; _orderSeasonalDiscount=orderSeasonalDiscount;
        }
        public void AddLine(OrderLine line)
        {
            if (Status != OrderStatus.Draft) throw new InvalidOperationException("Cannot modify lines when not in Draft");
            Lines.Add(line);
        }

        public void RecalcTotals()
        {
            Subtotal = Lines.Sum(l => _priceRule.ComputeLineAmount(l));
            CustomerDiscount = Subtotal * Customer.GetDiscountPercent(this);
            DiscountOnOrder = Subtotal*_orderDiscount.ComputeOrderLevelDiscount(this) + Subtotal * _orderSeasonalDiscount.ComputeOrderLevelDiscount(this);
            var taxable = Math.Max(0m, Subtotal - CustomerDiscount - DiscountOnOrder);
            VAT = _tax.ComputeTax(taxable);
            Total = taxable + VAT;//+FeeShip ;
        }

        public void ChangeStatus(OrderStatus newStatus)
        {
            if (newStatus == Status) return;
            var old = Status;
            Status = newStatus;
            OrderStatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(OrderNo, old, newStatus));
        }
        public void ApplyShippingFee(decimal fee)
        {
            Total += fee;
        }

    }
}
