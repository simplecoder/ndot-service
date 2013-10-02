using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using MongoRepository;
using Ndot.Models;

namespace Ndot.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly LogWriter _logger;
        private readonly IRepository<Sr1FormData> _repository;


        // GET api/values
        public ValuesController(LogWriter logger, IRepository<Sr1FormData> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        /*protected override void Dispose(bool disposing)
        {
            _logger.Dispose();
        }*/

        public IEnumerable<string> Get()
        {
            var form = new Sr1FormData
                {
                    City = "Las Vegas",
                    County = "Clark"
                };
            _repository.Add(form);
            /*using (var logger = new LogWriterFactory().Create())
            {
                logger.Write("Oh hi from values" + DateTime.Now);    
            }*/
            _logger.Write("Oh hi from values2" + DateTime.Now);    
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
    
}