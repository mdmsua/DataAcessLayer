﻿using DataAccessLayer.Core;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Services
{
    sealed class Dbo : IDbo
    {
        private readonly IDbCore _dbCore;

        public Dbo(IDbCore dbCore)
        {
            _dbCore = dbCore;
        }
        
        public IList<BillOfMaterials> GetBillOfMaterials(int startProductId, DateTime checkDate)
        {
            return _dbCore.ExecuteReader<BillOfMaterials>("uspGetBillOfMaterials", 
                DbParameters.Create(2)
                            .Set("StartProductID", startProductId)
                            .Set("CheckDate", checkDate)).ToList();
        }

        public Task<List<BillOfMaterials>> GetBillOfMaterialsAsync(int startProductId, DateTime checkDate)
        {
            return _dbCore.ExecuteReaderAsync<BillOfMaterials>("uspGetBillOfMaterials",
                DbParameters.Create(2)
                            .Set("StartProductID", startProductId)
                            .Set("CheckDate", checkDate)).ContinueWith(t => t.Result.ToList());
        }
    }
}
