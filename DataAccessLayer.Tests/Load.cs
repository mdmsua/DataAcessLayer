using DataAccessLayer.Core;
using DataAccessLayer.Entities;
using DataAccessLayer.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Tests
{
    [TestClass]
    public class Load
    {
        [TestMethod]
        public void Read_1K()
        {
            var actions = new List<Action>(1000);
            for (int i = 0; i < 1000; i++)
            {
                actions.Add(() => new UnitOfWork().Dbo.GetBillOfMaterials(1, new DateTime(2004, 7, 25)));
            }
            Parallel.ForEach(actions, action => action());
        }

        [TestMethod]
        public async Task ReadAsync_1K()
        {
            var actions = new List<Task<List<BillOfMaterials>>>(1000);
            for (int i = 0; i < 1000; i++)
            {
                actions.Add(new UnitOfWork().Dbo.GetBillOfMaterialsAsync(1, new DateTime(2004, 7, 25)));
            }
            await Task.WhenAll(actions);
        }

        [TestMethod]
        public void Scalar_1K()
        {
            var actions = new List<Action>(1000);
            for (int i = 0; i < 1000; i++)
            {
                actions.Add(() => new UnitOfWork().Scalar.Execute<int>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#")));
            }
            Parallel.ForEach(actions, action => action());
            
        }

        [TestMethod]
        public async Task ScalarAsync_1K()
        {
            var actions = new List<Task<int>>(1000);
            for (int i = 0; i < 1000; i++)
            {
                actions.Add(new UnitOfWork().Scalar.ExecuteAsync<int>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#")));
            }
            await Task.WhenAll(actions);
        }
    }
}
