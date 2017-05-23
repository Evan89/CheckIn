using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CheckInWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Home()
        {
            return new RedirectResult("https://safetylineloneworker.com/");
        }

        public ActionResult About()
        {
            return new RedirectResult("https://safetylineloneworker.com/tour/");
        }

        public ActionResult Contact()
        {
            return new RedirectResult("https://safetylineloneworker.com/contact-us/");
        }
    }
}