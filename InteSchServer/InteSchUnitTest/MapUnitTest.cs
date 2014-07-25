using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using InteSchBusiness;
using System.Fakes;
using Microsoft.QualityTools.Testing.Fakes;

namespace InteSchUnitTest
{
    [TestClass]
    public class MapUnitTest
    {
        [ClassInitialize()]
        public static void Prepare(TestContext test)
        {
            StudentTest.InitData("../../maps.xml");
            InteSchBusiness.Cache.InteSchCache cache = new InteSchBusiness.Cache.InteSchCache("127.0.0.1:11211");
            cache.Flush();
        }

        [TestMethod]
        [ExpectedException(typeof(InteSchBusiness.Error.ContentError))]
        public void InitMapTest()
        {
            Map map = new Map("125001");
            Assert.AreEqual(map.Id, "125001");
            Assert.AreEqual(map.Subject, "语文");

            map = new Map("12345");
        }

        [TestMethod]
        public void MapCacheTest()
        {
            Map map = new Map("125001");
            Assert.AreEqual(map.Subject, "语文");
        }
    }
}
