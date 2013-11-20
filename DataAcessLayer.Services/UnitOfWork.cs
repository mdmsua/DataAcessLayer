using DataAccessLayer.Core;
using DataAccessLayer.Services.Core;
using System;
using System.Data;

namespace DataAccessLayer.Services
{
    sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IDbCore _dbCore;

        private IHumanResources humanResources;

        private IScalar scalar;

        public UnitOfWork()
        {
            _dbCore = new DbCore();
        }

        public UnitOfWork(string name)
        {
            _dbCore = new DbCore(name);
        }
        
        public void BeginTransaction()
        {
            _dbCore.BeginTransaction();
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            _dbCore.BeginTransaction(isolationLevel);
        }
        
        public bool Commit(out Exception exception)
        {
            exception = null;
            
            return exception == null;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
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
