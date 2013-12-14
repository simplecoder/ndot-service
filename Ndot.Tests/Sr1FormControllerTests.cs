using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
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

        [TestMethod]
        public void GivenValidClientData_GeneratesValidSr1Form()
        {
            // Arrange
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/sr1form");
            var route = config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "sr1form" } });
            var mockRepo = new Mock<IRepository<Sr1FormData>>();
            mockRepo.Setup(c => c.Add(It.IsAny<Sr1FormData>())).Callback<Sr1FormData>(c => c.Id = "123");
            var mockEdmundsApi = new Mock<IEdmundsApiAgent>();
            mockEdmundsApi.Setup(c => c.GetVinData(It.IsAny<string>()))
                          .Returns(new VinApiData
                              {
                                  Year = "2003",
                                  Make = "Nissan",
                                  BodyType = "Car"
                              });
            var controller = new Sr1FormController(_logger, mockRepo.Object, mockEdmundsApi.Object);
            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            controller.Request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;

            // Act
            var result = controller.Post(GetValidSr1ClientFormData());

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
        }

        private Sr1ClientFormData GetValidSr1ClientFormData()
        {
            return new Sr1ClientFormData
                {
                    City = "Las Vegas",
                    Street = "123 Elm Street",
                    County = "Clark",
                    Actors = new List<ClientActor>
                        {
                            new ClientActor
                                {
                                    ActorType = "Driver",
                                    PlateNum = "888-RWC",
                                    PlateState = "NV",
                                    Vin = "3N1CB51D13L802315",
                                    DlBarCode = Convert.ToBase64String(File.ReadAllBytes("C:\\temp\\dlBarCode.JPG")),
                                    OwnerSameAsDriver = true
                                }
                        }
                };
        }

        /*[TestMethod]
        public void GivenValidClientData_GeneratesValidSr1Form()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("Default", "{controller}/{id}", new { id = RouteParameter.Optional });
            var server = new HttpServer(config);
            using (var client = new HttpMessageInvoker(server))
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/sr1form");
                request.Content = new StringContent("{\"Street\": \"123 Elm Street\", \"City\":\"Las Vegas\", \"County\":\"Clark\"}");
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //using (var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/sr1form"))
                    
                
                using (var response = client.SendAsync(request, CancellationToken.None).Result)
                {
                    Assert.Equals(HttpStatusCode.Created, response.StatusCode);
                }
            }
        }*/
    }
}
