using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SalesBicycleStore.Generics
{
    public class InMemoryRepository<T, TKey> : IRepository<T, TKey> where T : class
    {
        private readonly ConcurrentDictionary<TKey, T> _store = new ConcurrentDictionary<TKey, T>();
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
            return _store.TryGetValue(id, out var value) ? value : null;
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
            _store.TryRemove(id, out _);
        }
    }
}
