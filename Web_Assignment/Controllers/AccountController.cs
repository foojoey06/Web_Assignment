using Microsoft.AspNetCore.Mvc;

namespace Web_Assignment.Controllers
{
    public class AccountController : Controller
    {
        private readonly DB db;
        private readonly Helper hp;
        private readonly IWebHostEnvironment en;

        public AccountController(IWebHostEnvironment en, DB db, Helper hp)
        {
            this.db = db;
            this.en = en;
            this.hp = hp;
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
