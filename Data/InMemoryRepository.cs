using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BusinessEntities;

namespace Data
{
    public interface IRepository<T> where T : IdObject
    {
        T GetById(Guid id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(Guid id);
        void DeleteAll();
    }

    public class InMemoryRepository<T> : IRepository<T> where T : IdObject
    {
        private readonly ConcurrentDictionary<Guid, T> _entities = new ConcurrentDictionary<Guid, T>();

        public void Add(T entity)
        {
            if (!_entities.TryAdd(entity.Id, entity))
            {
                throw new InvalidOperationException($"Entity with ID {entity.Id} already exists");
            }
        }

        public void Update(T entity)
        {
            if (!_entities.TryUpdate(entity.Id, entity, _entities[entity.Id]))
            {
                throw new InvalidOperationException($"Entity with ID {entity.Id} does not exist");
            }
        }

        public void Delete(Guid id)
        {
            if (!_entities.TryRemove(id, out _))
            {
                throw new InvalidOperationException($"Entity with ID {id} does not exist");
            }
        }

        public T GetById(Guid id)
        {
            return _entities.TryGetValue(id, out var entity) ? entity : null;
        }

        public IEnumerable<T> GetAll()
        {
            return _entities.Values;
        }

        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            return _entities.Values.Where(predicate);
        }

        public void DeleteAll()
        {
            _entities.Clear();
        }
    }

    public class UserRepository : InMemoryRepository<User>
    {
        public IEnumerable<User> GetFiltered(string nameFilter = null, string emailFilter = null, decimal? minSalary = null, decimal? maxSalary = null)
        {
            var query = GetAll();

            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                query = query.Where(u => u.Name.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(emailFilter))
            {
                query = query.Where(u => u.Email.IndexOf(emailFilter, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (minSalary.HasValue)
            {
                query = query.Where(u => u.MonthlySalary >= minSalary.Value);
            }

            if (maxSalary.HasValue)
            {
                query = query.Where(u => u.MonthlySalary <= maxSalary.Value);
            }

            return query;
        }
    }

    public class ProductRepository : InMemoryRepository<Product>
    {
        public IEnumerable<Product> GetFiltered(string nameFilter = null, decimal? minPrice = null, decimal? maxPrice = null, string category = null)
        {
            var query = GetAll();

            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                query = query.Where(p => p.Name.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Categories.Any(c => c.Equals(category, StringComparison.OrdinalIgnoreCase)));
            }

            return query;
        }
    }

    public class OrderRepository : InMemoryRepository<Order>
    {
        public IEnumerable<Order> GetFiltered(Guid? userId = null, string status = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = GetAll();

            if (userId.HasValue)
            {
                query = query.Where(o => o.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= toDate.Value);
            }

            return query;
        }
    }
}
