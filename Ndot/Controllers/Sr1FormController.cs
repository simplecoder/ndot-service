using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MongoDB.Bson;
using MongoRepository;
using Ndot.Helpers;
using Ndot.Models;
using ZXing;

namespace Ndot.Controllers
{
    public class Sr1FormController : ApiController
    {
        private readonly LogWriter _logger;
        private readonly IRepository<Sr1FormData> _repository;
        private readonly IEdmundsApiAgent _apiAgent;

        public Sr1FormController(LogWriter logger, IRepository<Sr1FormData> repository, IEdmundsApiAgent apiAgent)
        {
            _logger = logger;
            _repository = repository;
            _apiAgent = apiAgent;
        }

        [Authorize]
        public IEnumerable<Sr1FormData> Get()
        {
            var results = _repository.Collection.FindAllAs<Sr1FormData>().ToList();
            results.Add(GetTestForm());
            return results;
        }

        [Authorize]
        public Sr1FormData Get(string id)
        {
            return _repository.Collection.FindOneByIdAs<Sr1FormData>(new ObjectId(id));
        }

        public HttpResponseMessage Post(Sr1ClientFormData clientFormData)
        {
            _logger.Write("New Form Received");

            ValidateClientFormData(clientFormData);

            var form = new Sr1FormData
                {
                    Street = clientFormData.Street,
                    City = clientFormData.City,
                    County = clientFormData.County,
                    CreatedDate = DateTime.Now,
                    Actors = new List<Actor>()
                };

            IBarcodeReader reader = new BarcodeReader
                {
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.PDF_417 }
                };

            foreach (var clientActor in clientFormData.Actors)
            {
                var actor = new Actor { Type = clientActor.ActorType, Driver = new DriverInfo(), Owner = new OwnerInfo() };
                if (!String.IsNullOrEmpty(clientActor.DlBarCode))
                {
                    var byteArray = Convert.FromBase64String(clientActor.DlBarCode);
                    var a = Image.FromStream(new MemoryStream(byteArray));
                    a.Save(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), Guid.NewGuid() + ".jpg"),
                                        ImageFormat.Jpeg);
                    var bmp = (Bitmap)a;

                    var barCodeDecoderResult = reader.Decode(bmp);
                    if (barCodeDecoderResult != null)
                    {
                        var dlData = DlBarCodeParser.Parse(barCodeDecoderResult.Text);
                        actor.Driver = GetDriverInfoFromDlData(dlData, clientActor);
                    }    
                }
                    

                if (!String.IsNullOrEmpty(clientActor.Vin))
                {
                    var vinData = _apiAgent.GetVinData(clientActor.Vin);
                    actor.Driver.Year = vinData.Year;
                    actor.Driver.Make = vinData.Make;
                    actor.Driver.BodyType = vinData.BodyType;    
                }
                    
                if (!clientActor.OwnerSameAsDriver && !String.IsNullOrEmpty(clientActor.DlBarCodeOwner))
                {
                    var byteArray = Convert.FromBase64String(clientActor.DlBarCodeOwner);
                    var bmp = (Bitmap)Image.FromStream(new MemoryStream(byteArray));
                    var barCodeDecoderResult = reader.Decode(bmp);
                    if (barCodeDecoderResult != null)
                    {
                        var dlData = DlBarCodeParser.Parse(barCodeDecoderResult.Text);
                        actor.Owner = GetOwnerInfoFromDlData(dlData);
                    }
                }
                form.Actors.Add(actor);
            }

            _repository.Add(form);

            var response = Request.CreateResponse(HttpStatusCode.Created, form);
            var url = Url.Link("DefaultApi", new {id = form.Id});
            response.Headers.Location = new Uri(url);
            return response;
        }

        private void ValidateClientFormData(Sr1ClientFormData clientFormData)
        {
            if (clientFormData == null)
                throw new HttpResponseException(HttpStatusCode.PreconditionFailed);

            if (String.IsNullOrEmpty(clientFormData.Street))
                throw new HttpResponseException(HttpStatusCode.PreconditionFailed);

            if (String.IsNullOrEmpty(clientFormData.City))
                throw new HttpResponseException(HttpStatusCode.PreconditionFailed);

            if (String.IsNullOrEmpty(clientFormData.County))
                throw new HttpResponseException(HttpStatusCode.PreconditionFailed);

            if (clientFormData.Actors == null || !clientFormData.Actors.Any())
                throw new HttpResponseException(HttpStatusCode.PreconditionFailed);
        }

        private static Sr1FormData GetTestForm()
        {
            return new Sr1FormData
                {
                    Id = "1232323",
                    Street = "123 Elm Street",
                    City = "Las Vegas",
                    County = "Clark",
                    CreatedDate = DateTime.Now,
                    Actors = new List<Actor>
                        {
                            new Actor
                                {
                                    Type = "Driver",
                                    Driver = new DriverInfo
                                        {
                                            FirstName = "John",
                                            MiddleName = "Z",
                                            LastName = "Xamarin",
                                            Street = "331 Main St.",
                                            City = "Las Vegas",
                                            State = "NV",
                                            Zip = "89129",
                                            DriverLicenseNumber = "124234422",
                                            DriverLicenseState = "NV",
                                            Dob = new DateTime(1990, 12, 3),
                                            LicensePlateNumber = "883GWN",
                                            LicensePlateState = "NV",
                                            Year = "2002",
                                            Make = "Nissan Sentra",
                                            BodyType = "Car",
                                            Vin = "231XJKJ9934KKJKJDKFJ"
                                        },
                                    Owner = new OwnerInfo
                                        {
                                            FirstName = "Bon",
                                            MiddleName = "Z",
                                            LastName = "Jonas",
                                            Street = "423 Zebas St.",
                                            City = "Las Vegas",
                                            State = "NV",
                                            Zip="89123",
                                            DriverLicenseNumber = "J2KJ2KJ3K",
                                            DriverLicenseState = "NV",
                                            Dob = new DateTime (1949, 4, 2)
                                        }
                                },
                                new Actor
                                {
                                    Type = "Pedestrian",
                                    OwnerSameAsDriver = true,
                                    Driver = new DriverInfo
                                        {
                                            FirstName = "Jenny",
                                            MiddleName = "Z",
                                            LastName = "Xamarin",
                                            Street = "331 Main St.",
                                            City = "Las Vegas",
                                            State = "NV",
                                            Zip = "89129",
                                            Dob = new DateTime(1990, 12, 3),
                                        }
                                }
                        }
                };
        }

        private static OwnerInfo GetOwnerInfoFromDlData(DlBarCodeData dlData)
        {
            return new OwnerInfo
                {
                    FirstName = dlData.FirstName,
                    MiddleName = dlData.MiddleInitial,
                    LastName = dlData.LastName,
                    Street = dlData.Street,
                    City = dlData.City,
                    State = dlData.State,
                    Zip = dlData.Zip,
                    DriverLicenseNumber = dlData.DriverLicenseNumber,
                    DriverLicenseState = dlData.DriverLicenseState,
                    Dob = dlData.Dob
                };
        }

        private static OwnerInfo CopyDriverInfoToOwnerInfo(Actor actor)
        {
            return new OwnerInfo
                {
                    FirstName = actor.Driver.FirstName,
                    MiddleName = actor.Driver.MiddleName,
                    LastName = actor.Driver.LastName,
                    Street = actor.Driver.Street,
                    City = actor.Driver.City,
                    State = actor.Driver.State,
                    Zip = actor.Driver.Zip,
                    DriverLicenseNumber = actor.Driver.DriverLicenseNumber,
                    DriverLicenseState = actor.Driver.DriverLicenseState,
                    Dob = actor.Driver.Dob
                };
        }

        private static DriverInfo GetDriverInfoFromDlData(DlBarCodeData dlData, ClientActor clientActor)
        {
            return new DriverInfo
                {
                    FirstName = dlData.FirstName,
                    MiddleName = dlData.MiddleInitial,
                    LastName = dlData.LastName,
                    Street = dlData.Street,
                    City = dlData.City,
                    State = dlData.State,
                    Zip = dlData.Zip,
                    DriverLicenseNumber = dlData.DriverLicenseNumber,
                    DriverLicenseState = dlData.DriverLicenseState,
                    Dob = dlData.Dob,
                    LicensePlateNumber = clientActor.PlateNum,
                    LicensePlateState = clientActor.PlateState
                };
        }
    }
}
