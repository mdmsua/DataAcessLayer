using DataAccessLayer.Core;

namespace DataAccessLayer.Services
{
    public interface IScalar
    {
        T Execute<T>(string procedure, DbParameters parameters);
    }
}
