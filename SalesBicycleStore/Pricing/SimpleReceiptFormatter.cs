using SalesBicycleStore.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBicycleStore.Pricing
{
    public class SimpleReceiptFormatter : IReceiptFormatter
    {
        private static readonly CultureInfo viVN = new CultureInfo("vi-VN");

        public string Format(Order order)
        {
            var sb = new StringBuilder();
            sb.AppendLine("==== HÓA ĐƠN BÁN Bicycle ====");
            sb.AppendLine($"Số đơn: {order.OrderNo}");
            sb.AppendLine($"Ngày   : {order.OrderDate:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"KH     : {order.Customer.FullName}");
            sb.AppendLine($"TT đơn : {order.Status}");
            sb.AppendLine("--------------------------");
            sb.AppendLine("Sản phẩm               SL    Đơn giá        Giảm%      Thành tiền");
            foreach (var l in order.Lines)
            {
                var lineAmount = l.LineAmount();
                sb.AppendLine(string.Format("{0,-20} {1,3} {2,12} {3,8:P0} {4,14}",
                    l.Product.Name.Length > 20 ? l.Product.Name.Substring(0, 20) : l.Product.Name,
                    l.Quantity,
                    l.Price.ToString("c0", viVN),
                    l.LineDiscountPercent,
                    lineAmount.ToString("c0", viVN)));
            }
            sb.AppendLine("--------------------------");
            sb.AppendLine($"Tạm tính : {order.Subtotal.ToString("c0", viVN)}");
            sb.AppendLine($"CK KH    : {order.CustomerDiscount.ToString("c0", viVN)}");
            sb.AppendLine($"CK Đơn   : {order.DiscountOnOrder.ToString("c0", viVN)}");
            sb.AppendLine($"VAT {order.VATPercent:P0} : {order.VAT.ToString("c0", viVN)}");
            //sb.AppendLine($"Phí ship Nếu Có: {order.FeeShip.ToString("c0",viVN)}");
            sb.AppendLine($"TỔNG CỘNG: {order.Total.ToString("c0", viVN)}");
            sb.AppendLine("==========================");
            return sb.ToString();
        }
    }
}
