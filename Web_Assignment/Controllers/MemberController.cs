using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Web_Assignment.Controllers
{
    public class MemberController : Controller
    {
        private readonly DB db;
        private readonly Helper hp;
        private readonly IWebHostEnvironment en;

        public MemberController(DB db, Helper hp, IWebHostEnvironment en)
        {
            this.en = en;
            this.hp = hp;
            this.db = db;
        }

        [Authorize(Roles = "Cashier")]
        public IActionResult Register()
        {
            return View();
        }

        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public IActionResult Register(RegisterVM vm, IFormFile photo)
        {
            //Validate Photo Only
            if (ModelState.IsValid)
            {
                var e = hp.ValidatePhoto(photo);
                if (e != "") ModelState.AddModelError("photo", e);
                TempData["Info"] = e;
            }

            if (ModelState.IsValid)
            {

                db.Staffs.Add(new()
                {
                    Name = vm.Name,
                    Password = hp.HashPassword(vm.Password),
                    Role = "Cashier",
                    Status = "Active",
                    Email = vm.Email,
                    Otp = 1, //TO DO
                    Path = "/member/" + hp.SavePhoto(photo, "member"),
                });
                db.SaveChanges();
                TempData["Info"] = $"Member {vm.Name} inserted.";
            }
            return View("Register");
        }
    }
}
