using System.Web.Mvc;

namespace SVDWebsite.Controllers
{
    public class AboutController : Controller
    {
        // GET: /About/
        public ActionResult Index()
        {
            return View();
        }

        // GET: /About/Terms/
        public ActionResult Terms()
        {
            return View();
        }

        // GET: /About/Privacy/
        public ActionResult Privacy()
        {
            return View();
        }

        // GET: /About/Reasoning/
        public ActionResult Reasoning()
        {
            return View();
        }

        // GET: /About/HelpUs/
        public ActionResult HelpUs()
        {
            return View();
        }
    }
}