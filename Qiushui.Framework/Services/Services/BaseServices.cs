using Qiushui.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Qiushui.Framework.Services
{
    public class BaseServices<TEntity> : IBaseServices<TEntity> where TEntity : class, new()
    {
        readonly IBaseRepository<TEntity> _baseRepository = new BaseRepository<TEntity>();

        public bool DeleteById(object objId)
        {
            return _baseRepository.DeleteById(objId);
        }

        public bool DeleteById(Expression<Func<TEntity, bool>> expression)
        {
            return _baseRepository.DeleteById(expression);
        }

        public bool Insert(TEntity entity)
        {
            return _baseRepository.Insert(entity);
        }

        public List<TEntity> Query(Expression<Func<TEntity, bool>> expression)
        {
            return _baseRepository.Query(expression);
        }

        public TEntity QueryById(object objId)
        {
            return _baseRepository.QueryById(objId);
        }

        public TEntity QueryById(Expression<Func<TEntity, bool>> expression)
        {
            return _baseRepository.QueryById(expression);
        }

        public bool Update(TEntity entity)
        {
            return _baseRepository.Update(entity);
        }
    }
}
