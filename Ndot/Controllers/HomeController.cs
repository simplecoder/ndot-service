using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using MongoRepository;
using Ndot.Models;

namespace Ndot.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Sr1FormData> _repository;

        public HomeController(IRepository<Sr1FormData> repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            ViewBag.GoogleMapsApiKey = ConfigurationManager.AppSettings["GoogleMapsApiKey"];
            var results = _repository.Collection.FindAllAs<Sr1FormData>().ToList();
            return View(results);
        }
    }
}
