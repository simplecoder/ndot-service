using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ndot.Helpers;

namespace Ndot.Tests
{
    [TestClass]
    public class EdmundsApiTests
    {
        [TestMethod]
        public void GivenVinForNissanSentra_GetsCorrectData()
        {
            IEdmundsApiAgent apiAgent = new EdmundsApiAgent();
            const string vin = "3N1CB51D13L802315";
            
            var data = apiAgent.GetVinData(vin);

            Assert.AreEqual("2003", data.Year);
            Assert.AreEqual("Nissan_Sentra", data.Make);
            Assert.AreEqual("Car", data.BodyType);
        }
    }
}
