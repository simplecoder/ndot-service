using System;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ndot.Controllers;

namespace Ndot.Tests
{
    [TestClass]
    public class ValidateDlImageControllerTests
    {
        [TestMethod]
        public void GivenValidDlImage_ReturnsTrue()
        {
            var logger = new LogWriter(new LoggingConfiguration());
            var image = Convert.ToBase64String(File.ReadAllBytes("C:\\temp\\workedonphone.JPG"));
            var controller = new ValidateDlImageController(logger);
            var result = controller.Post(image);
            Assert.IsTrue(result);
        }
    }
}
