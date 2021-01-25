using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Qiushui.Bot.Framework
{
    public class BaseServices<TEntity> : IBaseServices<TEntity> where TEntity : class, new()
    {
        IBaseRepository<TEntity> _baseRepository = new BaseRepository<TEntity>();

        public Task<bool> DeleteById(object objId)
        {
            return _baseRepository.DeleteById(objId);
        }

        public Task<bool> DeleteById(Expression<Func<TEntity, bool>> expression)
        {
            return _baseRepository.DeleteById(expression);
        }

        public Task<bool> Insert(TEntity entity)
        {
            return _baseRepository.Insert(entity);
        }

        public Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> expression)
        {
            return _baseRepository.Query(expression);
        }

        public Task<TEntity> QueryById(object objId)
        {
            return _baseRepository.QueryById(objId);
        }

        public Task<TEntity> QueryById(Expression<Func<TEntity, bool>> expression)
        {
            return _baseRepository.QueryById(expression);
        }

        public Task<bool> Update(TEntity entity)
        {
            return _baseRepository.Update(entity);
        }
    }
}
