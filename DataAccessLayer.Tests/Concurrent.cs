using DataAccessLayer.Core;
using DataAccessLayer.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Tests
{
    [TestClass]
    public class Concurrent
    {
        [TestMethod]
        public void All()
        {
            var length = 1000;
            var actions = new List<Action>(length);
            actions.AddRange(Enumerable.Range(1, length).Select(i => new Action(() =>
            {
                var uow = new UnitOfWork();
                using (var trx = uow.BeginTransaction())
                {
                    uow.Dbo.GetBillOfMaterials(3, new DateTime(2004, 7, 25));
                    uow.Scalar.Execute<int>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#"));
                }
            })));
            Parallel.Invoke(actions.ToArray());
        }

        [TestMethod]
        public async Task TaskWhenAll()
        {
            var uow = new UnitOfWork();
            await Task.WhenAll(uow.Dbo.GetBillOfMaterialsAsync(3, new DateTime(2004, 7, 25)), uow.Scalar.ExecuteAsync<int>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#")));
        }
    }
}
