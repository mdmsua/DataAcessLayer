using DataAccessLayer.Core;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Services.Core
{
    sealed class DbCore : IDbCore
    {
        private readonly Database _db;
        private readonly IList<string> _commands = new List<string>();
        private DbTransaction transaction = null;

        public DbCore(Database db)
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
            var rows = new List<T>();
            var mapper = MapBuilder<T>.BuildAllProperties();
            var command = PrepareCommand(procedure, parameters);
            using (var reader = DoExecuteReader(command))
            {
                while (reader.Read())
                {
                    rows.Add(mapper.MapRow(reader));
                }
            }
            return new ReadOnlyCollection<T>(rows);
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

        public DbTransaction Transaction
        {
            get
            {
                return transaction;
            }
            set
            {
                if (transaction != null) throw new InvalidOperationException();
                transaction = value;
                _commands.Clear();
            }
        }

        public IReadOnlyList<string> Commands
        {
            get
            {
                return new ReadOnlyCollection<string>(_commands);
            }
        }

        private bool IsPendingTransaction
        {
            get
            {
                return Transaction != null;
            }
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
            return IsPendingTransaction ? _db.ExecuteReader(command, transaction) : _db.ExecuteReader(command);
        }

        private int DoExecuteNonQuery(DbCommand command)
        {
            return IsPendingTransaction ? _db.ExecuteNonQuery(command, transaction) : _db.ExecuteNonQuery(command);
        }

        private object DoExecuteScalar(DbCommand command)
        {
            return IsPendingTransaction ? _db.ExecuteScalar(command, transaction) : _db.ExecuteScalar(command);
        }

        private void LogCommand(DbCommand command)
        {
            if (IsPendingTransaction)
                _commands.Add(Unwrap(command));
        }

        Func<DbCommand, String> Unwrap = command => String.Format("EXECUTE {0} {1}",
            command.CommandText,
            String.Join(",", from DbParameter p in command.Parameters select String.Format("{0}={1}", p.ParameterName, p.Value)).TrimEnd(','));
    }
}
