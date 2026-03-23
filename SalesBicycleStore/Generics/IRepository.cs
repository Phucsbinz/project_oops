using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesBicycleStore.Generics
{
    public interface IRepository<T, TKey> where T : class
    {
        T GetById(TKey id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Remove(TKey id);
    }
}

