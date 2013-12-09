using DataAccessLayer.Core;
using DataAccessLayer.Services.Core;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace DataAccessLayer.Services
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly DbCore _dbCore;

        private readonly DbConnection _dbConnection;

        private DbTransaction transaction;

        private IHumanResources humanResources;

        private IScalar scalar;

        private IDbo dbo;

        static UnitOfWork()
        {
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory(), false);
        }

        public UnitOfWork()
        {
            var db = DatabaseFactory.CreateDatabase() as SqlDatabase;
            _dbConnection = db.CreateConnection();
            _dbCore = new DbCore(db);
            _dbConnection.Open();
        }

        public UnitOfWork(string name)
        {
            var db = DatabaseFactory.CreateDatabase(name) as SqlDatabase;
            _dbConnection = db.CreateConnection();
            _dbCore = new DbCore(db);
            _dbConnection.Open();
        }
        
        public ITransactionManager UseTransaction()
        {
            transaction = _dbConnection.BeginTransaction();
            return _dbCore.Begin(transaction);
        }

        public ITransactionManager UseTransaction(IsolationLevel isolationLevel)
        {
            transaction = _dbConnection.BeginTransaction(isolationLevel);
            return _dbCore.Begin(transaction);
        }

        public bool TryCommit(out Exception exception)
        {
            exception = null;
            if (Monitor.TryEnter(_dbConnection))
            {
                if (transaction != null)
                {
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        exception = new Exception(_dbCore.Log, e);
                    }
                    finally
                    {
                        transaction.Dispose();
                        Monitor.Exit(_dbConnection);
                    }
                }
                return exception == null;
            }
            return false;
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

        public IDbo Dbo
        {
            get
            {
                if (dbo == null)
                    dbo = new Dbo(_dbCore);
                return dbo;
            }
        }
    }
}
