using Microsoft.AspNetCore.Mvc;

namespace Shoppers.Web.Mvc.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }


        public IActionResult Details()
        {
            return View();
        }
    }
}
