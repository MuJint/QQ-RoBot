using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Robot.Framework.Interface
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        private LiteDatabase _db;
        readonly IUnitWork _unitWork;
        public BaseRepository(IUnitWork unitWork)
        {
            _unitWork = unitWork;
            _db = _unitWork.GetDbClient();
        }

        public TEntity QueryById(object objId)
        {
            var table = _db.GetCollection<TEntity>();
            return table.FindById((BsonValue)objId);
        }

        public TEntity QueryById(Expression<Func<TEntity, bool>> expression)
        {
            var table = _db.GetCollection<TEntity>();
            return table.FindOne(expression);
        }

        public bool DeleteById(object objId)
        {
            var table = _db.GetCollection<TEntity>();
            return table.Delete((BsonValue)objId);
        }

        public bool DeleteById(Expression<Func<TEntity, bool>> expression)
        {
            var table = _db.GetCollection<TEntity>();
            return table.DeleteMany(expression) > 0;
        }

        public bool Insert(TEntity entity)
        {
            var table = _db.GetCollection<TEntity>();
            return table.Insert(entity) > 0;
        }

        public bool Update(TEntity entity)
        {
            var table = _db.GetCollection<TEntity>();
            return table.Update(entity);
        }

        public List<TEntity> Query(Expression<Func<TEntity, bool>> expression)
        {
            var table = _db.GetCollection<TEntity>();
            return table.Find(expression).ToList();
        }

        public int InsertR(TEntity entity)
        {
            var table = _db.GetCollection<TEntity>();
            return table.Insert(entity);
        }
    }
}
