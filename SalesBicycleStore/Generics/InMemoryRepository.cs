using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SalesBicycleStore.Generics
{
    public class InMemoryRepository<T, TKey> : IRepository<T, TKey> where T : class
    {
        private readonly Dictionary<TKey, T> _store = new Dictionary<TKey, T>();
        private readonly Func<T, TKey> _keySelector;

        public InMemoryRepository(Func<T, TKey> keySelector)
        {
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            _keySelector = keySelector;
        }

        public void Add(T entity)
        {
            var key = _keySelector(entity);
            _store[key] = entity;
        }

        public T GetById(TKey id)
        {
            T value;
            if (_store.TryGetValue(id, out value))
            {
                return value;
            }
            else { return null; }
        }

        public IEnumerable<T> GetAll()
        {
            return _store.Values;
        }

        public void Update(T entity)
        {
            var key = _keySelector(entity);
            _store[key] = entity;
        }

        public void Remove(TKey id)
        {
            _store.Remove(id);
        }
    }
}
