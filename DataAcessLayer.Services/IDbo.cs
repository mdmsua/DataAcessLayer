using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Services
{
    public interface IDbo
    {
        IList<BillOfMaterials> GetBillOfMaterials(int startProductId, DateTime checkDate);
        Task<List<BillOfMaterials>> GetBillOfMaterialsAsync(int startProductId, DateTime checkDate);
    }
}
