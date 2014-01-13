using DataAccessLayer.Core;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Services
{
    sealed class WriteOnly : IWriteOnly
    {
        private readonly IDbCore _dbCore;

        public WriteOnly(IDbCore dbCore)
        {
            _dbCore = dbCore;
        }
        
        public void Submit(ref long value)
        {
            var parameters = DbParameters.Create(1).Set("Value", value);
            _dbCore.ExecuteNonQuery("Submit", parameters);
            value = (long)parameters["Value"];
        }

        public void Rebind(long value)
        {
            var parameters = DbParameters.Create(1).Set("Value", value);
            _dbCore.ExecuteNonQuery("Rebind", parameters);
        }

        public void Submit(WriteValue writeValue)
        {
            var parameters = DbParameters.From(writeValue);
            _dbCore.ExecuteNonQuery("Submit", parameters);
            parameters.To(writeValue);
        }
    }
}
