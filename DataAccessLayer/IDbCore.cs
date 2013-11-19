using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DataAccessLayer.Core
{
    public interface IDbCore : IDisposable
    {
        T ExecuteScalar<T>(string procedure, DbParameters parameters);
        Task<T> ExecuteScalarAsync<T>(string procedure, DbParameters parameters);
        IReadOnlyList<T> ExecuteReader<T>(string procedure, DbParameters parameters) where T : new();
        Task<IReadOnlyList<T>> ExecuteReaderAsync<T>(string procedure, DbParameters parameters) where T : new();
        int ExecuteNonQuery(string procedure, DbParameters parameters);
        Task<int> ExecuteNonQueryAsync(string procedure, DbParameters parameters);
        void BeginTransaction();
        void BeginTransaction(IsolationLevel isolationLevel);
        bool Commit(out Exception exception);
    }
}
