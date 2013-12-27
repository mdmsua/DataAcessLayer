﻿using System.Threading.Tasks;

namespace DataAccessLayer.Core
{
    public interface IDbCore
    {
        T ExecuteScalar<T>(string procedure, DbParameters parameters);
        T[] ExecuteReader<T>(string procedure, DbParameters parameters) where T : new();
        int ExecuteNonQuery(string procedure, DbParameters parameters);

        Task<T> ExecuteScalarAsync<T>(string procedure, DbParameters parameters);
        Task<T[]> ExecuteReaderAsync<T>(string procedure, DbParameters parameters) where T : new();
        Task<int> ExecuteNonQueryAsync(string procedure, DbParameters parameters);
    }
}
