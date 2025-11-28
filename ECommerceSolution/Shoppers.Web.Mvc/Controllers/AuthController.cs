using Microsoft.AspNetCore.Mvc;

namespace Shoppers.Web.Mvc.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }


        public IActionResult Login()
        {
            return View();
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }


        public IActionResult Logout()
        {
            // ileride cookie/session temizlenecek
            return RedirectToAction("Login");
        }

    }

}
