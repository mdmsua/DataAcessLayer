using DataAccessLayer.Entities;

namespace DataAccessLayer.Services
{
    public interface IHumanResources
    {
        int UpdateEmployeeHireInfo(EmployeeHireInfo employeeHireInfo);
    }
}
