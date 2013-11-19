using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLayer.Core;

namespace DataAccessLayer.UnitTest
{
    [TestClass]
    public class ExecuteScalar
    {
        [TestMethod]
        public void Returns_3()
        {
            var result = new DbCore().ExecuteScalar<int>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#"));
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void Throws_InvalidCastException()
        {
            var result = new DbCore().ExecuteScalar<string>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#"));
            Assert.AreEqual(3, result);
        }
    }
}
