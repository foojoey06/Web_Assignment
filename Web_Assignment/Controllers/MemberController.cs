using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM vm, IFormFile photo)
        {
            //Validate Photo Only
            if (ModelState.IsValid)
            {
                var e = hp.ValidatePhoto(photo);
                if (e != "") ModelState.AddModelError("photo", e);
            }

            if (ModelState.IsValid)
            {

                db.Staffs.Add(new()
                {
                    Name = vm.Name,
                    Password = vm.Password,
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
