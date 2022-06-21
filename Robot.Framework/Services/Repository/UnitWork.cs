using LiteDB;
using Robot.Framework.Interface;
using System;

namespace Robot.Framework.Services
{
    public class UnitWork : IUnitWork
    {
        readonly LiteDatabase _liteDatabase;

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
    }
}
