using DataAccessLayer.Core;

namespace DataAccessLayer.Services
{
    sealed class WriteOnly : IWriteOnly
    {
        private readonly IDbCore _dbCore;

        public WriteOnly(IDbCore dbCore)
        {
            _dbCore = dbCore;
        }
        
        public void Submit(long value)
        {
            var parameters = DbParameters.Create(1).Set("Value", value);
            _dbCore.ExecuteNonQuery("Submit", parameters);
        }

        public void Rebind(long value)
        {
            var parameters = DbParameters.Create(1).Set("Value", value);
            _dbCore.ExecuteNonQuery("Rebind", parameters);
        }
    }
}
