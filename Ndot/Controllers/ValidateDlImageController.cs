using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Http;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using ZXing;

namespace Ndot.Controllers
{
    public class ValidateDlImageController : ApiController
    {
        private readonly LogWriter _logger;

        public ValidateDlImageController(LogWriter logger)
        {
            _logger = logger;
        }

        public bool Post([FromBody]string image)
        {
            _logger.Write(String.Format("Received request to validate driver license image. Datetime: {0}, Image:{1}" +
                                        "", DateTime.Now, image.Substring(0,10)));
            bool validationResult = false;
            try
            {
                if (string.IsNullOrEmpty(image))
                    return false;

                Result barCodeDecoderResult = null;
                IBarcodeReader reader = new BarcodeReader();
                reader.Options.TryHarder = true;
                reader.Options.PossibleFormats = new List<BarcodeFormat> {BarcodeFormat.PDF_417};
                var byteArray = Convert.FromBase64String(image);
                var a = Image.FromStream(new MemoryStream(byteArray));
                var bmp = (Bitmap) a ;
                /*bmp.Save(Guid.NewGuid() + ".jpg",ImageFormat.Jpeg);*/
                a.Save(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), Guid.NewGuid() + ".jpg"),
                       ImageFormat.Jpeg);
                barCodeDecoderResult = reader.Decode(bmp);

                validationResult = barCodeDecoderResult != null;
            }
            catch (Exception e)
            {
                _logger.Write(String.Format("Some Error occurred in ValidateDlImage. Exception: {0}", e));
            }

            _logger.Write(String.Format("Validation Result: {0}", validationResult));
            return validationResult;
        }
    }
}
