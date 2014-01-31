using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MongoDB.Bson;
using MongoRepository;
using Ndot.Helpers;
using Ndot.Hubs;
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

        [System.Web.Http.Authorize]
        public IEnumerable<Sr1FormData> Get()
        {
            var results = _repository.Collection.FindAllAs<Sr1FormData>().ToList();
            return results;
        }

        [System.Web.Http.Authorize]
        public Sr1FormData Get(string id)
        {
            return _repository.Collection.FindOneByIdAs<Sr1FormData>(new ObjectId(id));
        }

        public HttpResponseMessage Post(Sr1ClientFormData clientFormData)
        {
            _logger.Write("New Form Received");
            Sr1FormData form = null;
           
                ValidateClientFormData(clientFormData);

                form = new Sr1FormData
                    {
                        Street = clientFormData.Street,
                        City = clientFormData.City,
                        County = clientFormData.County,
                        Latitude = clientFormData.Latitude,
                        Longitude = clientFormData.Longitude,
                        CreatedDate = DateTime.Now,
                        Actors = new List<Actor>()
                    };

                IBarcodeReader reader = new BarcodeReader();
                reader.Options.TryHarder = true;
                reader.Options.PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.PDF_417 };

                foreach (var clientActor in clientFormData.Actors)
                {
                    var actor = new Actor { Type = clientActor.ActorType, Driver = new DriverInfo(), Owner = new OwnerInfo() };
                    if (clientActor.DlOverride)
                    {
                        actor.Driver = GetDriverInfoFromOverrideData(clientActor);
                    }
                    if (!String.IsNullOrEmpty(clientActor.DlBarCode))
                    {
                        var byteArray = Convert.FromBase64String(clientActor.DlBarCode);
                        var img = Image.FromStream(new MemoryStream(byteArray));
/*                        img.Save(
                            Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(),
                                         Guid.NewGuid() + ".jpg"),
                            ImageFormat.Jpeg);*/
                        var bmp = (Bitmap) img;

                        var barCodeDecoderResult = reader.Decode(bmp);
                        if (barCodeDecoderResult != null)
                        {
                            var dlData = DlBarCodeParser.Parse(barCodeDecoderResult.Text);
                            actor.Driver = GetDriverInfoFromDlData(dlData, clientActor);
                        }
                    }
                    else
                    {}
                    

                    if (!String.IsNullOrEmpty(clientActor.Vin))
                    {
                        var vinData = _apiAgent.GetVinData(clientActor.Vin);
                        actor.Driver.Vin = clientActor.Vin;
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

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<IncidentsHub>();
                hubContext.Clients.All.addNewMarkerToPage(form.Latitude, form.Longitude, 
                    form.Street, form.CreatedDate.ToString(), form.Actors.Count);
            

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

        
        private DriverInfo GetDriverInfoFromOverrideData(ClientActor clientActor)
        {
            var di = new DriverInfo
                {
                    FirstName = clientActor.FirstName,
                    MiddleName = clientActor.MiddleName,
                    LastName = clientActor.LastName,
                    Street = clientActor.Street,
                    City = clientActor.City,
                    State = clientActor.State,
                    Zip = clientActor.Zip
                };

            try
            {
                var dob = DateTime.Parse(clientActor.Dob);
                di.Dob = dob;
            }
            catch (Exception)
            {}
            return di;
        }

    }
}
