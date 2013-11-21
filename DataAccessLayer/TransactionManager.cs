using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DataAccessLayer.Core
{
    public abstract class TransactionManager
    {
        protected DbTransaction Transaction { get; private set; }

        protected IList<string> Statements;

        protected TransactionManager()
        {
            Statements = new List<string>();
        }

        protected bool IsPendingTransaction
        {
            get
            {
                return Transaction != null;
            }
        }
        
        public void Begin(DbTransaction transaction)
        {
            Transaction = transaction;
            Statements.Clear();
        }

        public IReadOnlyList<string> Commands
        {
            get
            {
                return new ReadOnlyCollection<string>(Statements);
            }
        }

        /*
        public void Commit()
        {
            try
            {
                Transaction.Commit();
            }
            catch (Exception)
            {
                Transaction.Rollback();
                throw;
            }
            finally
            {
                Transaction.Dispose();
            }
        }
        */
        
    }
}
