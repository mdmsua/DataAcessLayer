using DataAccessLayer.Core;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Services.Core
{
    sealed class DbCore : IDbCore
    {
        private readonly SqlDatabase _db;

        private readonly DbConnection _connection;

        private DbTransaction transaction;

        public DbCore(SqlDatabase db)
        {
            _db = db;
            _connection = _db.CreateConnection();
            if (_db.SupportsAsync)
                _connection.OpenAsync().Wait();
            else
                _connection.Open();
        }

        public DbTransaction BeginTransaction()
        {
            transaction = _connection.BeginTransaction();
            return transaction;
        }

        public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            transaction = _connection.BeginTransaction(isolationLevel);
            return transaction;
        }

        public T ExecuteScalar<T>(string procedure, DbParameters parameters)
        {
            var command = PrepareCommand(procedure, parameters);
            try
            {
                return (T)DoExecuteScalar(command);
            }
            catch (Exception exception)
            {
                throw new Exception(UnwrapCommand(command), exception);
            }
            finally
            {
                SetOutput(command, parameters);
            }
        }

        public Task<T> ExecuteScalarAsync<T>(string procedure, DbParameters parameters)
        {
            var command = PrepareCommand(procedure, parameters);
            try
            {
                if (_db.SupportsAsync)
                    return DoExecuteScalarAsync(command).ContinueWith(t => (T)t.Result);
                return Task.FromResult((T)DoExecuteScalar(command));
            }
            catch (Exception exception)
            {
                throw new Exception(UnwrapCommand(command), exception);
            }
            finally
            {
                SetOutput(command, parameters);
            }
        }

        public IEnumerable<T> ExecuteReader<T>(string procedure, DbParameters parameters) where T : new()
        {
            try
            {
                if (transaction == null)
                    return _db.ExecuteSprocAccessor<T>(procedure, parameters.Values.ToArray());
                return ExecuteReaderInternal<T>(procedure, parameters);
            }
            catch (Exception exception)
            {
                throw new Exception(UnwrapParameters(procedure, parameters), exception);
            }
        }

        public Task<IEnumerable<T>> ExecuteReaderAsync<T>(string procedure, DbParameters parameters) where T : new()
        {
            if (!_db.SupportsAsync) return Task.FromResult(ExecuteReader<T>(procedure, parameters));
            var accessor = _db.CreateSprocAccessor<T>(procedure);
            try
            {
                return Task.Factory.FromAsync<IEnumerable<T>>(accessor.BeginExecute(callback => { }, null, parameters.Values.ToArray()), accessor.EndExecute);
            }
            catch (Exception exception)
            {
                throw new Exception(UnwrapParameters(procedure, parameters), exception);
            }
        }

        public int ExecuteNonQuery(string procedure, DbParameters parameters)
        {
            var command = PrepareCommand(procedure, parameters);
            try
            {

                return DoExecuteNonQuery(command);
            }
            catch (Exception exception)
            {
                throw new Exception(UnwrapCommand(command), exception);
            }
            finally
            {
                SetOutput(command, parameters);
            }
        }

        public Task<int> ExecuteNonQueryAsync(string procedure, DbParameters parameters)
        {
            if (!_db.SupportsAsync) return Task.FromResult(ExecuteNonQuery(procedure, parameters));
            var command = PrepareCommand(procedure, parameters);
            try
            {
                return DoExecuteNonQueryAsync(command);
            }
            catch (Exception exception)
            {
                throw new Exception(UnwrapCommand(command), exception);
            }
            finally
            {
                SetOutput(command, parameters);
            }
        }

        private DbCommand PrepareCommand(string procedure, DbParameters parameters)
        {
            var command = _db.GetStoredProcCommand(procedure);
            if (_db.SupportsParemeterDiscovery)
                _db.DiscoverParameters(command);
            SetParameters(command, parameters);
            return command;
        }

        private void SetParameters(DbCommand command, IReadOnlyDictionary<string, object> parameters)
        {
            foreach (var parameter in parameters)
            {
                _db.SetParameterValue(command, _db.BuildParameterName(parameter.Key), parameter.Value);
            }
        }

        private void SetOutput(DbCommand command, DbParameters parameters)
        {
            var commandParameters =
                from DbParameter p in command.Parameters
                where p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output
                select p;
            foreach (var commandParameter in commandParameters)
            {
                var param = parameters.AsParallel().SingleOrDefault(p => string.Compare(_db.BuildParameterName(p.Key), commandParameter.ParameterName, true) == 0);
                if (param.Key == null) continue;
                parameters.Set(param.Key, param.Value, true);
            }
        }

        private IDataReader DoExecuteReader(DbCommand command)
        {
            return transaction == null ? _db.ExecuteReader(command) : _db.ExecuteReader(command, transaction);
        }

        private int DoExecuteNonQuery(DbCommand command)
        {
            return transaction == null ? _db.ExecuteNonQuery(command) : _db.ExecuteNonQuery(command, transaction);
        }

        private object DoExecuteScalar(DbCommand command)
        {
            return transaction == null ? _db.ExecuteScalar(command) : _db.ExecuteScalar(command, transaction);
        }

        private Task<IDataReader> DoExecuteReaderAsync(DbCommand command)
        {
            return transaction == null ?
                Task<IDataReader>.Factory.FromAsync<DbCommand>(_db.BeginExecuteReader, _db.EndExecuteReader, command, null) :
                Task<IDataReader>.Factory.FromAsync<DbCommand, DbTransaction>(_db.BeginExecuteReader, _db.EndExecuteReader, command, transaction, null);
        }

        private Task<int> DoExecuteNonQueryAsync(DbCommand command)
        {
            return transaction == null ?
                Task<int>.Factory.FromAsync<DbCommand>(_db.BeginExecuteNonQuery, _db.EndExecuteNonQuery, command, null) :
                Task<int>.Factory.FromAsync<DbCommand, DbTransaction>(_db.BeginExecuteNonQuery, _db.EndExecuteNonQuery, command, transaction, null);
        }

        private Task<object> DoExecuteScalarAsync(DbCommand command)
        {
            return transaction == null ?
                Task<object>.Factory.FromAsync<DbCommand>(_db.BeginExecuteScalar, _db.EndExecuteScalar, command, null) :
                Task<object>.Factory.FromAsync<DbCommand, DbTransaction>(_db.BeginExecuteScalar, _db.EndExecuteScalar, command, transaction, null);
        }

        private IEnumerable<T> ExecuteReaderInternal<T>(string procedure, DbParameters parameters) where T : new()
        {
            var command = PrepareCommand(procedure, parameters);
            var rowMapper = MapBuilder<T>.BuildAllProperties();
            using (var reader = DoExecuteReader(command))
            {
                while (reader.Read())
                {
                    yield return rowMapper.MapRow(reader);
                }
            }
        }

        Func<DbCommand, String> UnwrapCommand = command => String.Format("EXECUTE {0} {1}",
            command.CommandText, String.Join(", ",
            from DbParameter p in command.Parameters
            where p.Direction != ParameterDirection.ReturnValue
            select String.Format("{0}='{1}'", p.ParameterName, p.Value ?? (p.SourceColumnNullMapping ? null : string.Empty))));

        Func<String, DbParameters, String> UnwrapParameters = (procedure, parameters) => String.Format("EXECUTE {0} {1}",
            procedure, String.Join(", ",
            from p in parameters
            select String.Format("{0}='{1}'", p.Key, p.Value)));
    }
}
