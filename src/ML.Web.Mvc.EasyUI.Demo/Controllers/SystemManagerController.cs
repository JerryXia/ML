using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ML.Web.Mvc.EasyUI.Demo.Controllers
{
    public class SystemManagerController : Controller
    {

        [HttpPost]
        public ActionResult Log()
        {
            return View();
        }

    }
}
