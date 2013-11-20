using System;

namespace DataAccessLayer.Entities
{
    public class EmployeeHireInfo
    {
        public int BusinessEntityID { get; set; }

        public string JobTitle { get; set; }

        public DateTime HireDate { get; set; }

        public DateTime RateChangeDate { get; set; }

        public decimal Rate { get; set; }

        public byte PayFrequency { get; set; }

        public bool CurrentFlag { get; set; }
    }
}
