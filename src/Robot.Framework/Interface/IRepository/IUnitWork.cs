using LiteDB;
using System;

namespace Robot.Framework.Interface
{
    public interface IUnitWork : IDisposable
    {
        /// <summary>
        /// 获取Litedb链接
        /// </summary>
        /// <returns></returns>
        LiteDatabase GetDbClient();

        /// <summary>
        /// 开始事务
        /// </summary>
        void Begintran();

        /// <summary>
        /// 事务提交
        /// </summary>
        void Commit();

        /// <summary>
        /// 事务回滚
        /// </summary>
        void Rollback();
    }
}
