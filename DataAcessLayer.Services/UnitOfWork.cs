using DataAccessLayer.Services.Core;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;
using System.Data.Common;

namespace DataAccessLayer.Services
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly DbCore _dbCore;

        private readonly DbConnection _dbConnection;

        private IHumanResources humanResources;

        private IScalar scalar;

        private IDbo dbo;

        private IWriteOnly write;

        static UnitOfWork()
        {
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory(), false);
        }

        public UnitOfWork()
        {
            var db = DatabaseFactory.CreateDatabase() as SqlDatabase;
            _dbCore = new DbCore(db);
            _dbConnection = db.CreateConnection();
            _dbConnection.Open();
        }

        public UnitOfWork(string name)
        {
            var db = DatabaseFactory.CreateDatabase(name) as SqlDatabase;
            _dbCore = new DbCore(db);
            _dbConnection = db.CreateConnection();
            _dbConnection.Open();
        }
        
        public IDbTransaction BeginTransaction()
        {
            var transaction = _dbConnection.BeginTransaction();
            _dbCore.Transaction = transaction;
            return transaction;
        }

        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            var transaction = _dbConnection.BeginTransaction(isolationLevel);
            _dbCore.Transaction = transaction;
            return transaction;
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


        public IWriteOnly WriteOnly
        {
            get
            {
                if (write == null)
                    write = new WriteOnly(_dbCore);
                return write;
            }
        }
    }
}
