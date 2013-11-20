using DataAccessLayer.Core;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Services
{
    sealed class HumanResources : IHumanResources
    {
        private readonly IDbCore _dbCore;
        
        public HumanResources(IDbCore dbCore)
        {
            _dbCore = dbCore;
        }
        
        public int UpdateEmployeeHireInfo(EmployeeHireInfo employeeHireInfo)
        {
            return _dbCore.ExecuteNonQuery("uspUpdateEmployeeHireInfo", DbParameters.From<EmployeeHireInfo>(employeeHireInfo));
        }
    }
}
