using DataAccessLayer.Core;
using DataAccessLayer.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLayer.Tests
{
    [TestClass]
    public class Scalar
    {
        private IUnitOfWork repository;
        
        [TestInitialize]
        public void Initialize()
        {
            repository = new UnitOfWork();
        }

        [TestCleanup]
        public void Cleanup()
        {
            repository = null;
            GC.Collect();
        }
        
        [TestMethod]
        public void Get_Returns_Scalar_Value_3()
        {
            var result = repository.Scalar.Execute<int>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#"));
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void Get_Throws_InvalidCastException()
        {
            var result = repository.Scalar.Execute<string>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#"));
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public async Task GetAsync_Returns_Scalar_Value_3()
        {
            var result = await repository.Scalar.ExecuteAsync<int>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#"));
            Assert.AreEqual(3, result);
        }

        //[TestMethod]
        //public async Task GetAsync_With_Cancellation()
        //{
        //    var result = await repository.Scalar.ExecuteAsync<int>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#"), new CancellationTokenSource().Token);
        //    Assert.AreEqual(3, result);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(TaskCanceledException))]
        //public async Task GetAsync_Throws_TaskCanceledException()
        //{
        //    var result = await repository.ScalarAsync<int>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#"), new CancellationTokenSource(0).Token);
        //    Assert.AreEqual(3, result);
        //}

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public async Task GetAsync_Throws_InvalidCastException()
        {
            var result = await repository.Scalar.ExecuteAsync<string>("uspSearchCandidateResumes", DbParameters.Create(1).Set("searchString", "C#"));
            Assert.AreEqual(3, result);
        }
    }
}
