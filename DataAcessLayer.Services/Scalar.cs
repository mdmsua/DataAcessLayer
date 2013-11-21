using DataAccessLayer.Core;
using System.Threading.Tasks;

namespace DataAccessLayer.Services
{
    sealed class Scalar : IScalar
    {
        readonly IDbCore _dbCore;

        public Scalar(IDbCore dbCore)
        {
            _dbCore = dbCore;
        }
        
        public T Execute<T>(string procedure, DbParameters parameters)
        {
            return _dbCore.ExecuteScalar<T>(procedure, parameters);
        }


        public Task<T> ExecuteAsync<T>(string procedure, DbParameters parameters)
        {
            return _dbCore.ExecuteScalarAsync<T>(procedure, parameters);
        }
    }
}
