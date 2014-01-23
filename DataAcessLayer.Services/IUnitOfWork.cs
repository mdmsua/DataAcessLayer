using System;
using System.Data;

namespace DataAccessLayer.Services
{
    public interface IUnitOfWork
    {
        IDbTransaction BeginTransaction();
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel);

        IHumanResources HumanResources { get; }
        IScalar Scalar { get; }
        IDbo Dbo { get; }
        IWriteOnly WriteOnly { get; }
    }
}
