using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLayer.Services;
using DataAccessLayer.Entities;

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
                    var value = 1L;
                    uow.WriteOnly.Submit(ref value);
                    Assert.AreEqual(DateTime.Now.Year, value);
                }
            }
        }

        [TestMethod]
        public void WriteValue()
        {
            using (var uow = new UnitOfWork("WriteOnly"))
            {
                using (var trx = uow.BeginTransaction())
                {
                    var value = new WriteValue { Value = 1L };
                    uow.WriteOnly.Submit(value);
                    Assert.AreEqual(DateTime.Now.Year, value.Value);
                }
            }
        }
    }
}
