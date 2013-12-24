using System;
using System.Data;

namespace DataAccessLayer.Services
{
    public interface IUnitOfWork : IDisposable
    {
        IDbTransaction BeginTransaction();
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel);

        IHumanResources HumanResources { get; }
        IScalar Scalar { get; }
        IDbo Dbo { get; }
    }
}
