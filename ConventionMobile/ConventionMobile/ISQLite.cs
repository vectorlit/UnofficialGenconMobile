using System;
using SQLite;

namespace ConventionMobile
{
    public interface ISQLite
    {
        SQLiteAsyncConnection GetConnection();
        //SQLiteAsyncConnection DropAndRecreateThenGetConnection();
    }
}
