using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Threading;

namespace DataAccessLayer.Core
{
    public abstract class TransactionManager : ITransactionManager
    {
        protected DbTransaction Transaction { get; private set; }

        protected ConcurrentQueue<string> Commands { get; private set; }

        private readonly Object _lock = new object();

        protected TransactionManager()
        {
            Commands = new ConcurrentQueue<string>();
        }

        protected bool IsPendingTransaction
        {
            get
            {
                return Transaction != null;
            }
        }

        public ITransactionManager Begin(DbTransaction transaction)
        {
            if (Monitor.TryEnter(_lock))
            {
                if (!IsPendingTransaction)
                {
                    try
                    {
                        string result;
                        while (!Commands.IsEmpty)
                        {
                            Commands.TryDequeue(out result);
                        }
                        Transaction = transaction;
                    }
                    finally
                    {
                        Monitor.Exit(_lock);
                    }
                }
            }
            return this;
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


        public string Log
        {
            get { return string.Join(Environment.NewLine, Commands); }
        }

        public void Dispose()
        {
            
        }
    }
}
