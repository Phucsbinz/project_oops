using SalesBicycleStore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SalesBicycleStore.Domain
{
    public class OrderLine
    {
        public BicycleProduct Product { get; private set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineDiscountPercent { get; set; }
        public MaTerial Material { get; set; }
        public string TypeBicycle { get; set; } = "";
        public OrderLine(BicycleProduct product, int quantity, decimal price, decimal lineDiscountPercent,MaTerial material, string typeBicycle)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (quantity <= 0) throw new ArgumentException("Quantity must be > 0");
            if (price <= 0) throw new ArgumentException("UnitPrice must be > 0");
            if (lineDiscountPercent < 0m || lineDiscountPercent > 0.5m) throw new ArgumentException("LineDiscountPercent out of range [0..0.5]");
            if (typeBicycle == null) throw new ArgumentNullException(nameof(typeBicycle));
            if (material == null) throw new ArgumentNullException(nameof(material));
            Product = product;
            Quantity = quantity;
            Price = price;
            LineDiscountPercent = lineDiscountPercent;
            TypeBicycle=typeBicycle;
            Material=material;
        }
        public decimal LineAmount()
        {
            return Quantity * Price * (1 - LineDiscountPercent);
        }
    }
}
