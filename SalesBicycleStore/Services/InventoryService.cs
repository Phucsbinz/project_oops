using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SalesBicycleStore.Domain;
using SalesBicycleStore.Generics;

namespace SalesBicycleStore.Services
{
    public class InventoryService
    {
        private readonly IRepository<BicycleProduct, string> _productRepo;
        private readonly int _reorderThreshold;

        public event EventHandler<InventoryLowEventArgs> InventoryLow;

        public InventoryService(IRepository<BicycleProduct, string> productRepo, int reorderThreshold)
        {
            _productRepo = productRepo;
            _reorderThreshold = reorderThreshold;
        }

        public void DecreaseStock(string productId, int qty)
        {
            var p = _productRepo.GetById(productId);
            if (p == null) throw new InvalidOperationException("Product not found");
            if (p.StockQty < qty) throw new InvalidOperationException("Not enough stock");
            p.StockQty -= qty;
            _productRepo.Update(p);

            if (p.StockQty < _reorderThreshold)
            {
                var handler = InventoryLow;
                if (handler != null) handler(this, new InventoryLowEventArgs(p));
            }
        }

        public void IncreaseStock(string productId, int qty)
        {
            var p = _productRepo.GetById(productId);
            if (p == null) throw new InvalidOperationException("Product not found");
            p.StockQty += qty;
            _productRepo.Update(p);
        }
    }
}
