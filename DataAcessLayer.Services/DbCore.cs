using DataAccessLayer.Core;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Services.Core
{
    sealed class DbCore : TransactionManager, IDbCore
    {
        private readonly SqlDatabase _db;

        public DbCore(SqlDatabase db)
        {
            _db = db;
        }

        public T ExecuteScalar<T>(string procedure, DbParameters parameters)
        {
            var command = PrepareCommand(procedure, parameters);
            return (T)DoExecuteScalar(command);
        }

        public Task<T> ExecuteScalarAsync<T>(string procedure, DbParameters parameters)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<T> ExecuteReader<T>(string procedure, DbParameters parameters) where T : new()
        {
            return new ReadOnlyCollection<T>(_db.ExecuteSprocAccessor<T>(procedure, parameters.Values).ToList());
        }

        public Task<IReadOnlyList<T>> ExecuteReaderAsync<T>(string procedure, DbParameters parameters) where T : new()
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(string procedure, DbParameters parameters)
        {
            var command = PrepareCommand(procedure, parameters);
            return DoExecuteNonQuery(command);
        }

        public Task<int> ExecuteNonQueryAsync(string procedure, DbParameters parameters)
        {
            throw new NotImplementedException();
        }

        private DbCommand PrepareCommand(string procedure, DbParameters parameters)
        {
            var command = _db.GetStoredProcCommand(procedure);
            if (_db.SupportsParemeterDiscovery)
                _db.DiscoverParameters(command);
            SetParameters(command, parameters);
            LogCommand(command);
            return command;
        }

        private void SetParameter(DbCommand command, string name, object value)
        {
            var procParamName = _db.BuildParameterName(name);
            if (command.Parameters.Contains(procParamName))
            {
                DbType srcDbType, destDbType;
                destDbType = command.Parameters[procParamName].DbType;
                if (Enum.TryParse<DbType>(value.GetType().Name, out srcDbType) && srcDbType == destDbType)
                    _db.SetParameterValue(command, procParamName, value);
                else
                    throw new Exception();// ParameterTypeException(command.CommandText, procParamName, destDbType.ToString(), srcDbType.ToString());
            }
        }

        private void SetParameters(DbCommand command, IReadOnlyDictionary<string, object> parameters)
        {
            parameters.ToList().ForEach(p => SetParameter(command, p.Key, p.Value));
        }

        private IDataReader DoExecuteReader(DbCommand command)
        {
            return IsPendingTransaction ? _db.ExecuteReader(command, Transaction) : _db.ExecuteReader(command);
        }

        private int DoExecuteNonQuery(DbCommand command)
        {
            return IsPendingTransaction ? _db.ExecuteNonQuery(command, Transaction) : _db.ExecuteNonQuery(command);
        }

        private object DoExecuteScalar(DbCommand command)
        {
            return IsPendingTransaction ? _db.ExecuteScalar(command, Transaction) : _db.ExecuteScalar(command);
        }

        private void LogCommand(DbCommand command)
        {
            if (IsPendingTransaction)
                Statements.Add(Unwrap(command));
        }

        Func<DbCommand, String> Unwrap = command => String.Format("EXECUTE {0} {1}",
            command.CommandText,
            String.Join(",", from DbParameter p in command.Parameters select String.Format("{0}={1}", p.ParameterName, p.Value)).TrimEnd(','));
    }
}
