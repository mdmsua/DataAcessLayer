using System;

namespace DataAccessLayer.Core
{
    public interface ITransactionManager : IDisposable
    {
        String Log { get; }
    }
}
