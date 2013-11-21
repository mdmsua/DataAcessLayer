using DataAccessLayer.Core;
using System.Threading.Tasks;

namespace DataAccessLayer.Services
{
    public interface IScalar
    {
        T Execute<T>(string procedure, DbParameters parameters);
        Task<T> ExecuteAsync<T>(string procedure, DbParameters parameters);
    }
}
