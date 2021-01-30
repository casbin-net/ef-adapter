using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Casbin.NET.Adapter.EF;
using Casbin.NET.Adapter.EF.Model;
using EF_Adapter.Test;
using Moq;
using NetCasbin;
using Xunit;

namespace EF_Adapter.UnitTest
{
    public class AdapterTest : TestUtil
    {
        internal Mock<CasbinDbContext> _contextMock { get; set; }

        public AdapterTest()
        {
            _contextMock = new Mock<CasbinDbContext>("test-connectionString");

            InitPolicy();
        }

        private void InitPolicy()
        {
            var casbinRules = new List<CasbinRule>();
            casbinRules.Add(new CasbinRule()
            {
                PType = "p",
                V0 = "alice",
                V1 = "data1",
                V2 = "read",
            });
            casbinRules.Add(new CasbinRule()
            {
                PType = "p",
                V0 = "bob",
                V1 = "data2",
                V2 = "write",
            });
            casbinRules.Add(new CasbinRule()
            {
                PType = "p",
                V0 = "data2_admin",
                V1 = "data2",
                V2 = "read",
            });
            casbinRules.Add(new CasbinRule()
            {
                PType = "p",
                V0 = "data2_admin",
                V1 = "data2",
                V2 = "write",
            });
            casbinRules.Add(new CasbinRule()
            {
                PType = "g",
                V0 = "alice",
                V1 = "data2_admin",
            });

            var querableRules = casbinRules.AsQueryable();

            var mockSet = new Mock<DbSet<CasbinRule>>();

            mockSet.As<IQueryable<CasbinRule>>().Setup(m => m.Provider).Callback(() => querableRules = casbinRules.AsQueryable()).Returns(querableRules.Provider);
            mockSet.As<IQueryable<CasbinRule>>().Setup(m => m.Expression).Callback(() => querableRules = casbinRules.AsQueryable()).Returns(querableRules.Expression);
            mockSet.As<IQueryable<CasbinRule>>().Setup(m => m.ElementType).Callback(() => querableRules = casbinRules.AsQueryable()).Returns(querableRules.ElementType);
            mockSet.As<IQueryable<CasbinRule>>().Setup(m => m.GetEnumerator()).Callback(() => querableRules = casbinRules.AsQueryable()).Returns(querableRules.GetEnumerator());
            mockSet.Setup(x => x.AsNoTracking()).Callback(() => querableRules = casbinRules.AsQueryable()).Returns(mockSet.Object);


            _contextMock.Setup(c => c.CasbinRule).Returns(mockSet.Object);
            _contextMock.Setup(c => c.CasbinRule.Add(It.IsAny<CasbinRule>())).Callback<CasbinRule>(s =>
            {
                casbinRules.Add(s);
            });
            _contextMock.Setup(c => c.CasbinRule.RemoveRange(It.IsAny<IEnumerable<CasbinRule>>())).Callback<IEnumerable<CasbinRule>>(s =>
            {
                var items = s.ToList();
                foreach (var item in items)
                {
                    casbinRules.Remove(item);
                }
            });
        }

        [Fact]
        public void Test_Adapter_AutoSave()
        {

            var efAdapter = new CasbinDbAdapter(_contextMock.Object);
            Enforcer e = new Enforcer("examples/rbac_model.conf", efAdapter);
            var _context = _contextMock.Object;

            TestGetPolicy(e, AsList(
                AsList("alice", "data1", "read"),
                AsList("bob", "data2", "write"),
                AsList("data2_admin", "data2", "read"),
                AsList("data2_admin", "data2", "write")
            ));
            Assert.True(_context.CasbinRule.Count() == 5);

            e.AddPolicy("alice", "data1", "write");
            TestGetPolicy(e, AsList(
                AsList("alice", "data1", "read"),
                AsList("bob", "data2", "write"),
                AsList("data2_admin", "data2", "read"),
                AsList("data2_admin", "data2", "write"),
                AsList("alice", "data1", "write")
            ));
            Assert.True(_context.CasbinRule.Count() == 6);

            e.RemovePolicy("alice", "data1", "write");
            TestGetPolicy(e, AsList(
                AsList("alice", "data1", "read"),
                AsList("bob", "data2", "write"),
                AsList("data2_admin", "data2", "read"),
                AsList("data2_admin", "data2", "write")
            ));
            Assert.True(_context.CasbinRule.Count() == 5);

            e.RemoveFilteredPolicy(0, "data2_admin");
            TestGetPolicy(e, AsList(
                AsList("alice", "data1", "read"),
                AsList("bob", "data2", "write")
            ));
            Assert.True(_context.CasbinRule.Count() == 3);
        }
    }
}
