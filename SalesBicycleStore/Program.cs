using SalesBicycleStore.Domain;
using SalesBicycleStore.Generics;
using SalesBicycleStore.Pricing;
using SalesBicycleStore.Services;
using System.Globalization;
using System;

namespace SalesBicycleStore
{
    public class Program
    {
        static void Main(string[] args)
        {
            var viVN = new CultureInfo("vi-VN");
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("🚴‍♂️ SALES BICYCLE STORE DEMO\n");
            // 1️⃣ KHỞI TẠO REPOSITORY TRONG BỘ NHỚ
            var productRepo = new InMemoryRepository<BicycleProduct, string>(p => p.ProductId);
            var customerRepo = new InMemoryRepository<Customer, string>(c => c.CustomerId);
            var orderRepo = new InMemoryRepository<Order, string>(o => o.OrderNo);
            // 2️⃣ THÊM DỮ LIỆU SẢN PHẨM MẪU
            var bicycle1 = new BicycleProduct
            {
                ProductId = "BICYCLE-ROAD-01",
                Name = "Xe đua Carbon Ultra",
                Price = 25000000m,
                StockQty = 3,
                Material = MaTerial.Cacbon,
                TypeBicycle = "Xe đua"
            };
            var bicycle2 = new BicycleProduct
            {
                ProductId = "BICYCLE-MTB-02",
                Name = "Xe địa hình Steel Pro",
                Price = 15000000m,
                StockQty = 2,
                Material = MaTerial.Steel,
                TypeBicycle = "Xe địa hình"
            };
            var bicycle3 = new BicycleProduct
            {
                ProductId = "BICYCLE-CITY-03",
                Name = "Xe Đua Titan Premium",
                Price = 30000000m,
                StockQty = 1,
                Material = MaTerial.Titan,
                TypeBicycle = "Xe Đua"
            };
            productRepo.Add(bicycle1);
            productRepo.Add(bicycle2);
            productRepo.Add(bicycle3);
            // 3️⃣ TẠO KHÁCH HÀNG THÀNH VIÊN
            var vipMember = new MemberCustomers
            {
                FullName = "Nguyễn Văn A",
                Phone = "0901234567",
                MemberCode = "VIP001",
                Address =" Bình chiểu, Thủ Đức, Sài Gòn",
                Tier = MemberTier.Gold,
                Points = 10,
                DirectOrIndirect = true  // true = mua online, false = mua tại cửa hàng
            };
            customerRepo.Add(vipMember);
            // 4️⃣ CẤU HÌNH CHÍNH SÁCH GIÁ / THUẾ / HÓA ĐƠN
            IPriceRule priceRule = new DefaultPriceRule();
            IOrderDiscountPolicy orderDiscount = new NoOrderDiscount();
            IOrderDiscountPolicy orderSeasonalDiscount= new SeasonalDiscountPolicy();
            ITaxCalculator taxCalculator = new Vat8Percent();
            IReceiptFormatter receiptFormatter = new SimpleReceiptFormatter();
            // 5️⃣ KHỞI TẠO DỊCH VỤ
            var inventoryService = new InventoryService(productRepo, reorderThreshold: 2);
            var orderService = new OrderService(orderRepo, productRepo, customerRepo,
                                                priceRule, orderDiscount, orderSeasonalDiscount, taxCalculator,
                                                receiptFormatter, inventoryService);
            // 6️⃣ ĐĂNG KÝ SỰ KIỆN (EVENTS)
            orderService.OrderStatusChanged += (s, e) =>
            {
                Console.WriteLine($"[EVENT] Trạng thái đơn hàng: {e.OrderNo} {e.OldStatus} -> {e.NewStatus}");
            };
            inventoryService.InventoryLow += (s, e) =>
            {
                Console.WriteLine($"[EVENT] Hết hàng cảnh báo: {e.Product.Name} (Còn {e.Product.StockQty} chiếc)");
            };
            orderService.PointsAccrued += (s, e) =>
            {
                Console.WriteLine($"[EVENT] Tích điểm: {e.CustomerName} +{e.PointsAdded} điểm (Tổng: {e.NewPoints})");
            };
            orderService.BicycleExchanged += (s, e) =>
            {
                Console.WriteLine($"[EVENT] ĐỔI XE: {e.OldBicycleId} ({e.OldPrice:c0}) -> {e.NewBicycleId} ({e.NewPrice:c0})");
                if (e.Difference > 0)
                    Console.WriteLine($"Khách cần bù thêm: {e.Difference:c0}");
                else if (e.Difference < 0)
                    Console.WriteLine($"Khách được hoàn lại: {-e.Difference:c0}");
                else
                    Console.WriteLine("Đổi ngang giá, không phát sinh chênh lệch.");
            };
            orderService.MaterialChanged += (s, e) =>
            {
                Console.WriteLine($"[EVENT] ĐỔI VẬT LIỆU: {e.ProductId} từ {e.OldMaterial} sang {e.NewMaterial} — Giá mới: {e.NewPrice:c0}");
            };
            orderService.Ship += (s, e) =>
            {
                Console.WriteLine($"[EVENT] GIAO HÀNG ONLINE:");
                Console.WriteLine($"  ➜ Phí ship: {e.FeeShip:c0}");
                Console.WriteLine($"  ➜ Tổng trước ship: {e.OldTotal:c0}");
                Console.WriteLine($"  ➜ Tổng sau ship: {e.NewTotal:c0}");
                Console.WriteLine($"  ➜ địa chỉ gửi hàng: {e.Address}" );
            };
            orderService.TierUpgraded += (s, e) =>
            {
                Console.WriteLine($"[EVENT]  {e.CustomerName} đã được nâng hạng từ {e.OldTier} ➜ {e.NewTier}!");
            };
                // 7️⃣ TẠO ĐƠN HÀNG MỚI
                Console.WriteLine("\n--- 🧾 TẠO ĐƠN HÀNG ---");
            var order = orderService.CreateOrder(vipMember.CustomerId);
            orderService.AddLine(order.OrderNo, "BICYCLE-ROAD-01", qty: 1, lineDiscountPercent: 0.10m);
            orderService.AddLine(order.OrderNo, "BICYCLE-MTB-02", qty: 2, lineDiscountPercent: 0m);
            orderService.Recalc(order.OrderNo);
            Console.WriteLine(orderService.PrintReceipt(order.OrderNo));
            // 8️⃣ XÁC NHẬN & THANH TOÁN
            Console.WriteLine("\n---  XÁC NHẬN & THANH TOÁN ---");
            orderService.Confirm(order.OrderNo);
            orderService.Pay(order.OrderNo);
            Console.WriteLine("\n---  HOÁ ĐƠN SAU THANH TOÁN ---");
            Console.WriteLine(orderService.PrintReceipt(order.OrderNo));
            // 9️⃣ TEST: ĐỔI XE, ĐỔI VẬT LIỆU, SHIP,MUA TRẢ GÓP,NÂNG CẤP HẠNG THÀNH VIÊN
            Console.WriteLine("\n---  DEMO ĐỔI XE ---");
            orderService.ExchangeBicycle("BICYCLE-MTB-02", "BICYCLE-CITY-03");
            Console.WriteLine("\n---  DEMO ĐỔI VẬT LIỆU ---");
            orderService.ChangeMaterial("BICYCLE-ROAD-01", MaTerial.Titan);
            Console.WriteLine("\n---  DEMO SHIP ---");
            orderService.ShipBicycle(order.OrderNo);
            Console.WriteLine("\n---  DEMO MUA TRẢ GÓP CỦA 1 ĐƠN HÀNG BẤT KÌ  ---");
            Console.WriteLine($"Số Tiền Trả Góp Mỗi Tháng Của {bicycle3.Name} Là: {orderService.CalculateInstallment(bicycle3.Price, 5, 0.20m).ToString("c0", viVN)}");
            // 10️⃣ THỐNG KÊ
            Console.WriteLine("\n---  THỐNG KÊ ---");
            var calcTotalPrice = new TotalCalculator<OrderLine, decimal>(lines =>
            {
                decimal total = 0m;
                foreach (var l in lines)
                {
                    total += l.Price * l.Quantity;
                }
                return total;
            });
            Console.WriteLine($"[INFO] Tổng giá trị đơn hàng: {calcTotalPrice.Compute(order.Lines):c0}");
            Console.WriteLine("\n✅ Hoàn tất demo tất cả sự kiện! Nhấn phím bất kỳ để thoát...");
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey();
            }
        }
    }
}
