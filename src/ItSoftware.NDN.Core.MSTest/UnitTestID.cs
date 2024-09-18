using ItSoftware.Core.Extensions;
using ItSoftware.Core.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItSoftware.NDN.Core.MSTest
{
    [TestClass]
    public class UnitTestID
    {
        [TestMethod]
        public void TestItsID()
        {
            var value = ItsID.ItsCreateID(16, ItsCreateIDOptions.LowerAndUpperCase, true);            
            Assert.AreEqual(16, value.Length);            
        }
    }
}
