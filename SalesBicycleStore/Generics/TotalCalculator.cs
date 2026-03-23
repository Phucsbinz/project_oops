using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SalesBicycleStore.Generics
{
    public class TotalCalculator<TItem, TResult>
    {
        private readonly Func<IEnumerable<TItem>, TResult> _aggregate;
        public TotalCalculator(Func<IEnumerable<TItem>, TResult> aggregate)
        {
            _aggregate = aggregate;
        }
        public TResult Compute(IEnumerable<TItem> items) => _aggregate(items);
    }
}
