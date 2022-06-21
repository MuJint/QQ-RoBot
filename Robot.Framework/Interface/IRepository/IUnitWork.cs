using LiteDB;

namespace Robot.Framework.Interface
{
    public interface IUnitWork
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
