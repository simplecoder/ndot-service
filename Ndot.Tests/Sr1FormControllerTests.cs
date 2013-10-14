using System.Collections.Generic;
using System.Web.Http;
using MSTestExtensions;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoRepository;
using Moq;
using Ndot.Controllers;
using Ndot.Helpers;
using Ndot.Models;

namespace Ndot.Tests
{
    [TestClass]
    public class Sr1FormControllerTests
    {
        private readonly LogWriter _logger;

        public Sr1FormControllerTests()
        {
            _logger = new LogWriter(new LoggingConfiguration());
        }

        [TestMethod]
        public void GivenNullForm_ThrowsPreconditionFailed()
        {
            var mockRepo = new Mock<IRepository<Sr1FormData>>();
            var mockEdmundsApi = new Mock<IEdmundsApiAgent>();
            var controller = new Sr1FormController(_logger, mockRepo.Object, mockEdmundsApi.Object);
            ExceptionAssert.Throws<HttpResponseException>(() => controller.Post(null));
        }

        [TestMethod]
        public void GivenNullOrEmptyStreet_ThrowsPreconditionFailed()
        {
            var mockRepo = new Mock<IRepository<Sr1FormData>>();
            var mockEdmundsApi = new Mock<IEdmundsApiAgent>();
            var controller = new Sr1FormController(_logger, mockRepo.Object, mockEdmundsApi.Object);
            var form = new Sr1ClientFormData(){City = "Las Vegas", County = "Clark", Actors = new List<ClientActor>{new ClientActor()}};
            ExceptionAssert.Throws<HttpResponseException>(() => controller.Post(form));
        }

        [TestMethod]
        public void GivenNullOrEmptyCity_ThrowsPreconditionFailed()
        {
            var mockRepo = new Mock<IRepository<Sr1FormData>>();
            var mockEdmundsApi = new Mock<IEdmundsApiAgent>();
            var controller = new Sr1FormController(_logger, mockRepo.Object, mockEdmundsApi.Object);
            var form = new Sr1ClientFormData() { Street = "123 Elm St", County = "Clark", Actors = new List<ClientActor> { new ClientActor() } };
            ExceptionAssert.Throws<HttpResponseException>(() => controller.Post(form));
        }

        [TestMethod]
        public void GivenNullOrEmptyCounty_ThrowsPreconditionFailed()
        {
            var mockRepo = new Mock<IRepository<Sr1FormData>>();
            var mockEdmundsApi = new Mock<IEdmundsApiAgent>();
            var controller = new Sr1FormController(_logger, mockRepo.Object, mockEdmundsApi.Object);
            var form = new Sr1ClientFormData() { Street = "123 Elm St", City = "Las Vegas", Actors = new List<ClientActor> { new ClientActor() } };
            ExceptionAssert.Throws<HttpResponseException>(() => controller.Post(form));
        }

        [TestMethod]
        public void GivenNullOrEmptyActors_ThrowsPreconditionFailed()
        {
            var mockRepo = new Mock<IRepository<Sr1FormData>>();
            var mockEdmundsApi = new Mock<IEdmundsApiAgent>();
            var controller = new Sr1FormController(_logger, mockRepo.Object, mockEdmundsApi.Object);
            var form = new Sr1ClientFormData() { Street = "123 Elm St",City = "Las Vegas", County = "Clark", Actors = new List<ClientActor>()};
            ExceptionAssert.Throws<HttpResponseException>(() => controller.Post(form));
        }
    }
}
