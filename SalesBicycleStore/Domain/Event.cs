using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace SalesBicycleStore.Domain
{
    public class OrderStatusChangedEventArgs : EventArgs
    {
        public string OrderNo { get; }
        public OrderStatus OldStatus { get; }
        public OrderStatus NewStatus { get; }
        public OrderStatusChangedEventArgs(string orderNo, OrderStatus oldS, OrderStatus newS)
        {
            OrderNo = orderNo; OldStatus = oldS; NewStatus = newS;
        }
    }

    public class InventoryLowEventArgs : EventArgs
    {
        public BicycleProduct Product { get; }
        public InventoryLowEventArgs(BicycleProduct p) { Product = p; }
    }

    public class PointsAccruedEventArgs : EventArgs
    {
        public string CustomerId { get; }
        public string CustomerName { get; }
        public int PointsAdded { get; }
        public int NewPoints { get; }
        public PointsAccruedEventArgs(string customerId, string customerName, int added, int newPoints)
        {
            CustomerId = customerId; CustomerName = customerName;
            PointsAdded = added; NewPoints = newPoints;
        }
    }
    // thêm event chọn hoặc đổi chất liệu xe
    public class MaterialChangedEventArgs : EventArgs
    {
        public string ProductId { get; }
        public MaTerial OldMaterial { get; }
        public MaTerial NewMaterial { get; }
        public decimal NewPrice { get; }

        public MaterialChangedEventArgs(string productId, MaTerial oldMat, MaTerial newMat, decimal newPrice)
        {
            ProductId = productId;
            OldMaterial = oldMat;
            NewMaterial = newMat;
            NewPrice = newPrice;
        }
    }
    public class BicycleExchangedEventArgs : EventArgs
    {
        public string OldBicycleId { get; }
        public string NewBicycleId { get; }
        public decimal OldPrice { get; }
        public decimal NewPrice { get; }
        public decimal Difference { get; }

        public BicycleExchangedEventArgs(string oldId, string newId, decimal oldPrice, decimal newPrice, decimal diff)
        {
            OldBicycleId = oldId;
            NewBicycleId = newId;
            OldPrice = oldPrice;
            NewPrice = newPrice;
            Difference = diff;
        }
    }
    /*public class ShipBicycleEventArgs : EventArgs
    {
        public decimal FeeShip { get; }
        public decimal NewPrice { get; }
        public decimal Price { get; }

        public ShipBicycleEventArgs(decimal feeShip, decimal price)
          {
              FeeShip = feeShip;
              Price = price;
              NewPrice = FeeShip+Price;
          }  
      }*/
    public class ShipBicycleEventArgs : EventArgs
    {
        public decimal FeeShip { get; }
        public decimal OldTotal { get; }
        public decimal NewTotal { get; }
        public string Address {  get; }
        public ShipBicycleEventArgs(decimal feeShip, decimal oldTotal, decimal newTotal,string address)
        {
            FeeShip = feeShip;
            OldTotal = oldTotal;
            NewTotal = newTotal;
            Address=address;
        }
    }
    public class TierUpgradedEventArgs : EventArgs
    {
        public string CustomerId { get; }
        public string CustomerName { get; }
        public MemberTier OldTier { get; }
        public MemberTier NewTier { get; }

        public TierUpgradedEventArgs(string id, string name, MemberTier oldTier, MemberTier newTier)
        {
            CustomerId = id;
            CustomerName = name;
            OldTier = oldTier;
            NewTier = newTier;
        }
    }

}
