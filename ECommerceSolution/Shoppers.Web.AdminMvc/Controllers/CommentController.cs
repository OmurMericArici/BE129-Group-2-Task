using Microsoft.AspNetCore.Mvc;

namespace Shoppers.Web.AdminMvc.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult List()
        {
            return View();
        }

        public IActionResult Approve()
        {
            return View();
        }
    }
}
