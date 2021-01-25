using LiteDB;
using Qiushui.Bot.Framework.Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Qiushui.Bot.Framework
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        internal LiteDatabase _db;
        public BaseRepository()
        {
            _db = UnitOfWork.GetDbClient;
        }

        public Task<TEntity> QueryById(object objId)
        {
            return Task.Run(() =>
             {
                 var table = _db.GetCollection<TEntity>();
                 return table.FindById((BsonValue)objId);
             });
        }

        public Task<TEntity> QueryById(Expression<Func<TEntity, bool>> expression)
        {
            return Task.Run(() =>
            {
                var table = _db.GetCollection<TEntity>();
                return table.FindOne(expression);
            });
        }

        public Task<bool> DeleteById(object objId)
        {
            return Task.Run(() =>
            {
                var table = _db.GetCollection<TEntity>();
                return table.Delete((BsonValue)objId);
            });
        }

        public Task<bool> DeleteById(Expression<Func<TEntity, bool>> expression)
        {
            return Task.Run(() =>
            {
                var table = _db.GetCollection<TEntity>();
                return table.DeleteMany(expression) > 0;
            });
        }

        public Task<bool> Insert(TEntity entity)
        {
            return Task.Run(() =>
            {
                var table = _db.GetCollection<TEntity>();
                return table.Insert(entity) > 0;
            });
        }

        public Task<bool> Update(TEntity entity)
        {
            return Task.Run(() =>
            {
                var table = _db.GetCollection<TEntity>();
                return table.Update(entity);
            });
        }

        public Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> expression)
        {
            return Task.Run(() =>
            {
                var table = _db.GetCollection<TEntity>();
                return table.Find(expression).ToList();
            });
        }
    }
}
