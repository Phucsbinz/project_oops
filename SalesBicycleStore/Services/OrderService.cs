using SalesBicycleStore.Domain;
using SalesBicycleStore.Generics;
using SalesBicycleStore.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBicycleStore.Services
{
    public class OrderService
    {
        // fee ship cố định là 300k
        public decimal FeeShip { get; set; } = 300000m;
        private readonly IRepository<Order, string> _orderRepo;
        private readonly IRepository<BicycleProduct, string> _productRepo;
        private readonly IRepository<Customer, string> _customerRepo;
        private readonly IPriceRule _priceRule;
        private readonly IOrderDiscountPolicy _orderDiscount;
        private readonly IOrderDiscountPolicy _orderSeasonalDiscount;
        private readonly ITaxCalculator _tax;
        private readonly IReceiptFormatter _receiptFormatter;
        private readonly InventoryService _inventory;
        public event EventHandler<OrderStatusChangedEventArgs> OrderStatusChanged;
        public event EventHandler<PointsAccruedEventArgs> PointsAccrued;
        public event EventHandler<TierUpgradedEventArgs> TierUpgraded;
        public OrderService(IRepository<Order, string> orderRepo,
                            IRepository<BicycleProduct, string> productRepo,
                            IRepository<Customer, string> customerRepo,
                            IPriceRule priceRule,
                            IOrderDiscountPolicy orderDiscount,
                            IOrderDiscountPolicy orderSeasonalDiscount,
                            ITaxCalculator tax,
                            IReceiptFormatter receiptFormatter,
                            InventoryService inventory)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _customerRepo = customerRepo;
            _orderSeasonalDiscount= orderSeasonalDiscount;
            _priceRule = priceRule;
            _orderDiscount = orderDiscount;
            _tax = tax;
            _receiptFormatter = receiptFormatter;
            _inventory = inventory;
        }
        public Order CreateOrder(string customerId)
        {
            var customer = _customerRepo.GetById(customerId);
            if (customer == null) throw new InvalidOperationException("Customer not found");

            var order = new Order(customer, _priceRule, _orderDiscount, _orderSeasonalDiscount, _tax);
            order.OrderStatusChanged += (s, e) =>
            {
                var handler = OrderStatusChanged;
                if (handler != null) handler(this, e);
            };

            _orderRepo.Add(order);
            return order;
        }
        public void AddLine(string orderNo, string productId, int qty, decimal lineDiscountPercent)
        {
            var order = _orderRepo.GetById(orderNo);
            if (order == null) throw new InvalidOperationException("Order not found");
            var product = _productRepo.GetById(productId);
            if (product == null) throw new InvalidOperationException("Product not found");
            var line = new OrderLine(product, qty, product.Price, lineDiscountPercent, product.Material, product.TypeBicycle);
            order.AddLine(line);
            _orderRepo.Update(order);
        }
        public void Recalc(string orderNo)
        {
            var order = _orderRepo.GetById(orderNo);
            if (order == null) throw new InvalidOperationException("Order not found");
            order.RecalcTotals();
            _orderRepo.Update(order);
        }
        public void Confirm(string orderNo)
        {
            var order = _orderRepo.GetById(orderNo);
            if (order == null) throw new InvalidOperationException("Order not found");
            if (order.Status != OrderStatus.Draft) throw new InvalidOperationException("Only Draft can be Confirmed");
            // Kiểm tra tồn & trừ kho
            foreach (var l in order.Lines)
            {
                _inventory.DecreaseStock(l.Product.ProductId, l.Quantity);
            }
            order.ChangeStatus(OrderStatus.Confirmed);
            _orderRepo.Update(order);
        }
        public void Pay(string orderNo)
        {
            var order = _orderRepo.GetById(orderNo);
            if (order == null) throw new InvalidOperationException("Order not found");
            if (order.Status != OrderStatus.Confirmed) throw new InvalidOperationException("Only Confirmed can be Paid");
            // Tính lại để chắc chắn số liệu
            order.RecalcTotals();
            _orderRepo.Update(order);
            // Tích điểm cho MemberCustomer
            var member = order.Customer as MemberCustomers;
            if (member != null)
            {
                // Taxable = Subtotal - CustDiscount - OrderDiscount
                var taxable = Math.Max(0m, order.Subtotal - order.CustomerDiscount - order.DiscountOnOrder);
                var pointsAdded = (int)Math.Floor(taxable / 100000m); // 100k = 1 điểm
                member.Points += pointsAdded;
                _customerRepo.Update(member);
                var handler = PointsAccrued;
                if (handler != null) handler(this, new PointsAccruedEventArgs(member.CustomerId, member.FullName, pointsAdded, member.Points));
            }
            order.ChangeStatus(OrderStatus.Paid);
            _orderRepo.Update(order);
            var oldTier = member.Tier;
            if (member.Points >= 300)
                member.Tier = MemberTier.Platinum;
            else if (member.Points >= 150)
                member.Tier = MemberTier.Gold;
            else if (member.Points >= 50)
                member.Tier = MemberTier.Silver;
            else
                member.Tier = MemberTier.Standard;
            if (member.Tier != oldTier)
            {
                TierUpgraded?.Invoke(this, new TierUpgradedEventArgs(
                    member.CustomerId, member.FullName, oldTier, member.Tier
                ));
            }
        }
        public string PrintReceipt(string orderNo)
        {
            var order = _orderRepo.GetById(orderNo);
            if (order == null) throw new InvalidOperationException("Order not found");
            return _receiptFormatter.Format(order);
        }
        public event EventHandler<BicycleExchangedEventArgs> BicycleExchanged;
        public void ExchangeBicycle(string oldProductId, string newProductId)
        {
            var oldBicycle = _productRepo.GetById(oldProductId);
            var newBicycle = _productRepo.GetById(newProductId);
            if (oldBicycle == null || newBicycle == null)
                throw new InvalidOperationException("Không tìm thấy sản phẩm để đổi.");
            decimal diff = newBicycle.Price - oldBicycle.Price;
            // Cập nhật tồn kho: cộng xe cũ (vì khách hàng trả lại), trừ xe mới (vì bán ra)
            _inventory.IncreaseStock(oldBicycle.ProductId, 1);
            _inventory.DecreaseStock(newBicycle.ProductId, 1);
            // Bắn event thông báo đổi xe
            BicycleExchanged?.Invoke(this, new BicycleExchangedEventArgs(
                oldBicycle.ProductId, newBicycle.ProductId, oldBicycle.Price, newBicycle.Price, diff
            ));
        }
        public event EventHandler<MaterialChangedEventArgs> MaterialChanged;
        public void ChangeMaterial(string productId, MaTerial newMaterial)
        {
            var product = _productRepo.GetById(productId);
            if (product == null) throw new InvalidOperationException("Không tìm thấy xe để đổi vật liệu.");
            var oldMat = product.Material;
            if (oldMat == newMaterial) return;
            // Tăng giá theo vật liệu (tùy loại)
            switch (newMaterial)
            {
                case MaTerial.Aluminum: product.Price *= 1.00m; break;
                case MaTerial.Steel: product.Price *= 1.05m; break;
                case MaTerial.Cacbon: product.Price *= 1.10m; break;
                case MaTerial.Titan: product.Price *= 1.15m; break;
            }
            product.Material = newMaterial;
            _productRepo.Update(product);

            MaterialChanged?.Invoke(this, new MaterialChangedEventArgs(product.ProductId, oldMat, newMaterial, product.Price));
        }
        /*public void ShipBicycle(Customer customer,BicycleProduct product)
        {
            if (customer.DirectOrIndirect == false)
            {
                return;
            }
            Ship?.Invoke(this, new ShipBicycleEventArgs(FeeShip, product.Price));
        }*/
        public event EventHandler<ShipBicycleEventArgs> Ship;
        public void ShipBicycle(string orderNo)
        {
            var order = _orderRepo.GetById(orderNo);
            if (order == null)
                throw new InvalidOperationException("Không tìm thấy đơn hàng để giao.");
            var customer = order.Customer;
            if (customer == null)
                throw new InvalidOperationException("Đơn hàng không có thông tin khách hàng.");
            // Nếu mua trực tiếp => không áp dụng phí ship
            if (customer.DirectOrIndirect == false)
            {
                Console.WriteLine("[INFO] Khách hàng mua trực tiếp, không áp dụng phí ship.");
                return;
            }
            // Ngược lại, tính phí ship cho khách online
            var shippingFee = FeeShip;
            var totalBeforeShip = order.Total;
            var totalAfterShip = totalBeforeShip + shippingFee;
            string address = customer.Address;
            // Bắn event
            Ship?.Invoke(this, new ShipBicycleEventArgs(shippingFee, totalBeforeShip, totalAfterShip,address));
            // Cập nhật lại tổng (nếu muốn lưu vào đơn hàng)
            order.ApplyShippingFee(shippingFee);
            _orderRepo.Update(order);
        }
         public decimal CalculateInstallment(decimal total, int  months,decimal interestRate)
        {
            if (months < 3) throw new ArgumentOutOfRangeException("số tháng phải => 3");
            decimal totalWithInterest = total * (1 + interestRate); //total*(1+rate) công thức lãi suất
            // Số tiền mỗi tháng phải trả
            decimal monthlyPayment = totalWithInterest / months;
            return Math.Round(monthlyPayment, 0, MidpointRounding.AwayFromZero);
        }
    }
}
