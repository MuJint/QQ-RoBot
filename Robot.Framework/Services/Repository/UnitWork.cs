using LiteDB;
using Robot.Framework.Interface;
using System;

namespace Robot.Framework.Services
{
    public class UnitWork : IUnitWork, IDisposable
    {
        private LiteDatabase _liteDatabase;
        private bool disposedValue;

        public UnitWork()
        {
            _liteDatabase = new LiteDatabase($"{Environment.CurrentDirectory}\\Main.db");
        }

        public void Begintran()
        {
            lock (this)
            {
                GetDbClient().BeginTrans();
            }
        }

        public void Commit()
        {
            lock (this)
            {
                try
                {
                    GetDbClient().Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    GetDbClient().Rollback();
                }
            }
        }

        public LiteDatabase GetDbClient()
        {
            return _liteDatabase ?? new LiteDatabase($"{Environment.CurrentDirectory}\\Main.db");
        }

        public void Rollback()
        {
            lock (this)
            {
                GetDbClient().Rollback();
            }
        }

        #region 释放

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                _liteDatabase = null;
                disposedValue = true;
            }
        }

        // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~UnitWork()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
