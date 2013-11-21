using DbRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLayer.Tests
{
    [TestClass]
    public class Read
    {
        private DbRepository.IDbRepository repository;

        [TestInitialize]
        public void Initialize()
        {
            repository = new DbRepository.DbRepository();
        }

        [TestCleanup]
        public void Cleanup()
        {
            repository = null;
            GC.Collect();
        }
        
        [TestMethod]
        public void Read_Is_Not_Null()
        {
            var results = repository.Read<BillOfMaterials>("uspGetBillOfMaterials", Parameters);
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void Read_Is_Instance_Of_Type()
        {
            var results = repository.Read<BillOfMaterials>("uspGetBillOfMaterials", Parameters);
            Assert.IsInstanceOfType(results, typeof(IEnumerable<BillOfMaterials>));
        }

        [TestMethod]
        public void Read_Has_3_Elements()
        {
            var results = repository.Read<BillOfMaterials>("uspGetBillOfMaterials", Parameters);
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void Read_First_Element_Equals_Reference()
        {
            var results = repository.Read<BillOfMaterials>("uspGetBillOfMaterials", Parameters);
            Assert.ReferenceEquals(Reference, results.First());
        }

        [TestMethod]
        public async Task ReadAsync_Is_Not_Null()
        {
            var results = await repository.ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", Parameters);
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task ReadAsync_Is_Instance_Of_Type()
        {
            var results = await repository.ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", Parameters);
            Assert.IsInstanceOfType(results, typeof(IEnumerable<BillOfMaterials>));
        }

        [TestMethod]
        public async Task ReadAsync_Has_3_Elements()
        {
            var results = await repository.ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", Parameters);
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public async Task ReadAsync_First_Element_Equals_Reference()
        {
            var results = await repository.ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", Parameters);
            Assert.ReferenceEquals(Reference, results.First());
        }

        [TestMethod]
        public async Task ReadAsync_With_Cancellation()
        {
            var results = await repository.ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", Parameters, new CancellationTokenSource().Token);
            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task ReadAsync_Throws_TaskCanceledException()
        {
            await repository.ReadAsync<BillOfMaterials>("uspGetBillOfMaterials", Parameters, new CancellationTokenSource(0).Token);
        }

        [TestMethod]
        [ExpectedException(typeof(ParameterTypeException))]
        public void Read_Throws_ParameterTypeException()
        {
            var result = repository.Read<BillOfMaterials>("uspGetBillOfMaterials", ThrowingParameterTypeException);
        }

        static BillOfMaterials Reference
        {
            get
            {
                return new BillOfMaterials
                {
                    ProductAssemblyID = 3,
                    ComponentID = 461,
                    ComponentDesc = "Lock Ring",
                    TotalQuantity = 1,
                    StandardCost = 0,
                    ListPrice = 0,
                    BOMLevel = 3,
                    RecursionLevel = 0
                };
            }
        }

        static Parameters Parameters
        {
            get
            {
                return Parameters.Create(2).Set("StartProductID", 3).Set("CheckDate", new DateTime(2004, 7, 25));
            }
        }

        static Parameters ThrowingParameterTypeException
        {
            get
            {
                return Parameters.Create(2).Set("StartProductID", new Exception()).Set("CheckDate", new DateTime(2004, 7, 25));
            }
        }
    }
}
