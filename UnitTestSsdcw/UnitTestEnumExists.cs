using Microsoft.VisualStudio.TestTools.UnitTesting;
using ssdcw.Controllers;
using ssdcw.Models;

namespace UnitTestSsdcw
{
    [TestClass]
    public class UnitTestEnumExists
    {
        [TestMethod]
        public void TestStatusEnumExists()
        {
            string enumName = "Open";
            bool resultActual;
            bool resultExpected = true;
            
            resultActual = Ticket.StatusEnumExists(enumName);

            Assert.AreEqual(resultExpected, resultActual, "Test passed");

        }

        [TestMethod]
        public void TestTypeEnumExists()
        {
            string enumName = "Development";
            bool resultActual;
            bool resultExpected = true;

            resultActual = Ticket.TypeEnumExists(enumName);

            Assert.AreEqual(resultExpected, resultActual, "Test passed");

        }

        [TestMethod]
        public void TestPriorityEnumExists()
        {
            string enumName = "Medium";
            bool resultActual;
            bool resultExpected = true;

            resultActual = Ticket.PriorityEnumExists(enumName);

            Assert.AreEqual(resultExpected, resultActual, "Test passed");

        }
    }
}
