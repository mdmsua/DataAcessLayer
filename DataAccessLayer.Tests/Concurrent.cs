using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLayer.Services;
using DataAccessLayer.Core;
using System.Threading.Tasks;

namespace DataAccessLayer.Tests
{
    [TestClass]
    public class Concurrent
    {
        [TestMethod]
        public void All()
        {
            var uow = new UnitOfWork();
            Parallel.Invoke(() =>
            {
                using (uow.UseTransaction())
                {
                    uow.Dbo.GetBillOfMaterials(3, new DateTime(2004, 7, 25));
                    uow.Scalar.Execute<int>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#"));
                }
            },
            () =>
            {
                uow.Dbo.GetBillOfMaterials(3, new DateTime(2004, 7, 25));
                uow.Scalar.Execute<int>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#"));
            });
        }
    }
}
