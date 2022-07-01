using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Robot.Framework.Interface
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        List<TEntity> Query(Expression<Func<TEntity, bool>> expression);
        TEntity QueryById(object objId);
        TEntity QueryById(Expression<Func<TEntity, bool>> expression);
        bool DeleteById(int objId);
        bool Delete(Expression<Func<TEntity, bool>> expression);
        bool Insert(TEntity entity);
        int InsertR(TEntity entity);
        bool Update(TEntity entity);
    }
}
