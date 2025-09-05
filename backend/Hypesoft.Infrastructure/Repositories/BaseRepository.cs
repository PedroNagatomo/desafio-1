using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hypesoft.Domain.Entities;
using Hypesoft.Domain.Repositories;
using MongoDB.Driver;

namespace Hypesoft.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;

        protected BaseRepository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _collection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(predicate).AnyAsync(cancellationToken);
        }

        public virtual async Task<long> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate == null
                ? await _collection.CountDocumentsAsync(FilterDefinition<T>.Empty, cancellationToken: cancellationToken)
                : await _collection.CountDocumentsAsync(predicate, cancellationToken: cancellationToken);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            var entitiesList = entities.ToList();
            if (entitiesList.Any())
            {
                await _collection.InsertManyAsync(entitiesList, cancellationToken: cancellationToken);
            }
            return entitiesList;
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity.UpdateTimestamp();
            await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: cancellationToken);
        }

        public virtual async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(entity.Id, cancellationToken);
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            var ids = entities.Select(x => x.Id).ToList();
            if (ids.Any())
            {
                await _collection.DeleteManyAsync(x => ids.Contains(x.Id), cancellationToken);
            }
        }
    }
}