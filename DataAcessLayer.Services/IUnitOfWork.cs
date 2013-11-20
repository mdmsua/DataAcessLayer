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
        void BeginTransaction();
        void BeginTransaction(IsolationLevel isolationLevel);
        bool Commit(out Exception exception);

        IHumanResources HumanResources { get; }
        IScalar Scalar { get; }
    }
}
