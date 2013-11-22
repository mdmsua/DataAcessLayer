using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Services
{
    public interface IUnitOfWork : IDisposable
    {
        void UseTransaction();
        void UseTransaction(IsolationLevel isolationLevel);
        bool TryCommit(out Exception exception);

        IHumanResources HumanResources { get; }
        IScalar Scalar { get; }
        IDbo Dbo { get; }
    }
}
