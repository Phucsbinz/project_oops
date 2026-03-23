using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBicycleStore.Domain
{
    public class BicycleProduct
    {
        public string ProductId { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string Grade { get; set; } = "";
        public decimal WeightKg { get; set; }
        // đổi unit price sang price là giá mỗi chiếc xe
        public decimal Price { get; set; }
        public int StockQty { get; set; }
        public bool IsActive { get; set; } = true;
        // kích cỡ 
        public string FrameSize { get; set; } = "";
        // thêm loại xe 
        public string TypeBicycle { get; set; } = "";
        // thêm chất liệu của xe nhôm sắt cacbon....
        public MaTerial Material { get; set; }
    }
}

