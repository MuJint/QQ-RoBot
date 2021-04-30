using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Qiushui.Framework.Interface
{
    public interface IBaseServices<TEntity> where TEntity : class
    {
        List<TEntity> Query(Expression<Func<TEntity, bool>> expression);
        TEntity QueryById(object objId);
        TEntity QueryById(Expression<Func<TEntity, bool>> expression);
        bool DeleteById(object objId);
        bool DeleteById(Expression<Func<TEntity, bool>> expression);
        bool Insert(TEntity entity);
        bool Update(TEntity entity);
    }
}
