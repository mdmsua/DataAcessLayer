using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Core
{
    public sealed class DbCore : IDbCore
    {
        private readonly Database _db;
        private readonly DbConnection connection;
        private DbTransaction transaction;
        private IDictionary<string, DbParameters> commands;

        static DbCore()
        {
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory(), false);
        }

        public DbCore()
        {
            _db = DatabaseFactory.CreateDatabase();
            connection = _db.CreateConnection();
        }

        public DbCore(string name)
        {
            _db = DatabaseFactory.CreateDatabase(name);
            connection = _db.CreateConnection();
        }

        public T ExecuteScalar<T>(string procedure, DbParameters parameters)
        {
            var command = GetCommand(procedure, parameters);
            return (T)DoExecuteScalar(command);
        }

        public Task<T> ExecuteScalarAsync<T>(string procedure, DbParameters parameters)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<T> ExecuteReader<T>(string procedure, DbParameters parameters) where T : new()
        {
            var mapper = MapBuilder<T>.BuildAllProperties();
            var list = new List<T>();
            var command = GetCommand(procedure, parameters);
            using (var reader = DoExecuteReader(command))
            {
                while (reader.Read())
                {
                    list.Add(mapper.MapRow(reader));
                }
            }
            return new ReadOnlyCollection<T>(list);
        }

        public Task<IReadOnlyList<T>> ExecuteReaderAsync<T>(string procedure, DbParameters parameters) where T : new()
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(string procedure, DbParameters parameters)
        {
            var command = GetCommand(procedure, parameters);
            return DoExecuteNonQuery(command);
        }

        public Task<int> ExecuteNonQueryAsync(string procedure, DbParameters parameters)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (IsPendingTransaction)
                transaction.Rollback();
            if (connection != null)
                connection.Close();
            transaction.Dispose();
            connection.Dispose();
        }

        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.Serializable);
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (transaction != null)
                throw new InvalidOperationException("The transaction is already in progress");
            if (connection.State == ConnectionState.Open)
                transaction = connection.BeginTransaction(isolationLevel);
            else
                throw new InvalidExpressionException("The connection is broken");
            commands = new Dictionary<string, DbParameters>();
        }

        public bool Commit(out Exception exception)
        {
            exception = null;
            if (transaction != null)
            {
                try
                {
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    exception = new Exception(string.Empty, e);
                }
                finally
                {
                    commands.Clear();
                    commands = null;
                }
            }
            return exception == null;
        }

        private bool IsPendingTransaction
        {
            get
            {
                return transaction != null;
            }
        }

        private DbCommand GetCommand(string procedure, DbParameters parameters)
        {
            var command = _db.GetStoredProcCommand(procedure);
            if (_db.SupportsParemeterDiscovery)
                _db.DiscoverParameters(command);
            SetParameters(command, parameters);
            if (IsPendingTransaction)
                commands.Add(procedure, parameters);
            return command;
        }

        private void PrepareCommand(DbCommand command, DbConnection connection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (connection == null) throw new ArgumentNullException("connection");

            command.Connection = connection;
        }

        private void PrepareCommand(DbCommand command, DbTransaction transaction)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (transaction == null) throw new ArgumentNullException("connection");

            PrepareCommand(command, transaction.Connection);
            command.Transaction = transaction;
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
    }
}
