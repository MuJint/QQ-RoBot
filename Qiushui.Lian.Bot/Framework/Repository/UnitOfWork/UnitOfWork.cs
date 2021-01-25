using LiteDB;
using System;

namespace Qiushui.Lian.Bot.Framework.Repository.UnitOfWork
{
    public class UnitOfWork
    {
        private static LiteDatabase _liteDatabase = null;
        private static readonly object _lock = new object();

        public UnitOfWork()
        {

        }

        public static LiteDatabase GetDbClient
        {
            get
            {
                lock (_lock)
                {
                    return _liteDatabase ??= new LiteDatabase($"{Environment.CurrentDirectory}\\Main.db");
                }
            }
        }
    }
}
