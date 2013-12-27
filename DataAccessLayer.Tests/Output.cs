using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLayer.Services;

namespace DataAccessLayer.Tests
{
    [TestClass]
    public class Output
    {
        [TestMethod]
        public void Write()
        {
            using (var uow = new UnitOfWork("WriteOnly"))
            {
                using (var trx = uow.BeginTransaction())
                {
                    var value = 1;
                    uow.WriteOnly.Submit(value);
                    Assert.AreEqual(DateTime.Now.Year, value);
                }
            }
        }
    }
}
