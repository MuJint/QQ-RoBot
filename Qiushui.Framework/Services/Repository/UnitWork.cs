using LiteDB;
using System;

namespace Qiushui.Framework.Services
{
    public class UnitWork
    {
        static LiteDatabase _liteDatabase = null;
        static readonly object _lock = new();

        #region Func
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
        #endregion
    }
}
