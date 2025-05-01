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

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM vm, string? returnURL)
        {
            //Get User record based on email
            var user = db.Staffs.FirstOrDefault(user => user.Email == vm.Email);

            //Verify Password
            if(user == null || !hp.VerifyPassword(user.Password, vm.Password))
            {
                ModelState.AddModelError("", "Incorrect Login Credentials.");
            }

            if (ModelState.IsValid)
            {
                TempData["Info"] = "Login Successfully.";

                //Sign in
                hp.SignIn(user!.Email, user.Role, vm.RememberMe);

                //Handle Return URL
                if (string.IsNullOrEmpty(returnURL))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(vm);
        }

        public IActionResult Logout(string? returnURL)
        {
            TempData["Info"] = "Logout successfully.";

            // Sign out
            hp.SignOut();

            return RedirectToAction("Index", "Home");
        }

    }
}
