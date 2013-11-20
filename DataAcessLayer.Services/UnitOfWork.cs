using DataAccessLayer.Core;
using DataAccessLayer.Services.Core;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data;
using System.Data.Common;

namespace DataAccessLayer.Services
{
    sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IDbCore _dbCore;

        private readonly DbConnection _dbConnection;

        private IHumanResources humanResources;

        private IScalar scalar;

        static UnitOfWork()
        {
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory(), false);
        }

        public UnitOfWork()
        {
            var db = DatabaseFactory.CreateDatabase();
            _dbConnection = db.CreateConnection();
            _dbCore = new DbCore(db);
        }

        public UnitOfWork(string name)
        {
            var db = DatabaseFactory.CreateDatabase(name);
            _dbConnection = db.CreateConnection();
            _dbCore = new DbCore(db);
        }
        
        public void BeginTransaction()
        {
            _dbCore.Transaction = _dbConnection.BeginTransaction();
        }
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            _dbCore.Transaction = _dbConnection.BeginTransaction(isolationLevel);
        }

        public bool Commit(out Exception exception)
        {
            exception = null;
            if (_dbCore.Transaction != null)
            {
                try
                {
                    _dbCore.Transaction.Commit();
                }
                catch (Exception e)
                {
                    _dbCore.Transaction.Rollback();
                    exception = new Exception(String.Join(Environment.NewLine, _dbCore.Commands), e);
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
