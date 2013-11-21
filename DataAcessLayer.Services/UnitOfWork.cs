using DataAccessLayer.Services.Core;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace DataAccessLayer.Services
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly DbCore _dbCore;

        private readonly DbConnection _dbConnection;

        private DbTransaction transaction;

        private IHumanResources humanResources;

        private IScalar scalar;

        static UnitOfWork()
        {
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory(), false);
        }

        public UnitOfWork()
        {
            var db = DatabaseFactory.CreateDatabase() as SqlDatabase;
            _dbConnection = db.CreateConnection();
            _dbCore = new DbCore(db);
        }

        public UnitOfWork(string name)
        {
            var db = DatabaseFactory.CreateDatabase(name) as SqlDatabase;
            _dbConnection = db.CreateConnection();
            _dbCore = new DbCore(db);
        }
        
        public void UseTransaction()
        {
            transaction = _dbConnection.BeginTransaction();
            _dbCore.Begin(transaction);
        }

        public void UseTransaction(IsolationLevel isolationLevel)
        {
            transaction = _dbConnection.BeginTransaction(isolationLevel);
            _dbCore.Begin(transaction);
        }

        public bool TryCommit(out Exception exception)
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
                    exception = new Exception(String.Join(Environment.NewLine, _dbCore.Commands), e);
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            return exception == null;
        }
        
        public void Dispose()
        {
            _dbConnection.Dispose();
        }

        public IHumanResources HumanResources
        {
            get 
            {  
                if (humanResources == null)
                    humanResources = new HumanResources(_dbCore);
                return humanResources;
            }
        }

        public IScalar Scalar
        {
            get
            {
                if (scalar == null)
                    scalar = new Scalar(_dbCore);
                return scalar;
            }
        }
    }
}
