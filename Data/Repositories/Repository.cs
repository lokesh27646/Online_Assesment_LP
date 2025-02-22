using System;
using System.Collections.Generic;
using System.Linq;
using BusinessEntities;
using Common;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Indexes;

namespace Data.Repositories
{
    [AutoRegister]
    public class Repository<T> : IRepository<T> where T : IdObject
    {
        private readonly IDocumentSession _documentSession;

        public Repository(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public void Add(T entity)
        {
            _documentSession.Store(entity);
        }

        public void Update(T entity)
        {
            _documentSession.Store(entity);
        }

        public void Delete(Guid id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _documentSession.Delete(entity);
            }
        }

        public T GetById(Guid id)
        {
            return _documentSession.Load<T>(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _documentSession.Query<T>();
        }

        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            return _documentSession.Query<T>().Where(predicate);
        }

        protected void DeleteAll<TIndex>() where TIndex : AbstractIndexCreationTask<T>
        {
            _documentSession.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex(typeof(TIndex).Name, new IndexQuery());
        }
    }
}