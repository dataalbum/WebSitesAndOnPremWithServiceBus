using System.Web.Mvc;
using WebSitesWithOnPremIntegration.Core;

namespace WebSitesWithOnPremIntegration.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Customer customer)
        {
            ServiceBusHelper.Setup().Publish(customer);
            return View(new Customer());
        }
    }
}
