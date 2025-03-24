
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web_Assignment.Controllers;

public class AdminController : Controller
{
    private readonly IWebHostEnvironment en;
    private readonly DB db;
    private readonly Helper hp;

    public AdminController(IWebHostEnvironment en, DB db, Helper hp)
    {
        this.db = db;
        this.en = en;
        this.hp = hp;
    }

    public IActionResult Index()
    {
        ViewBag.Staffs = db.Staffs;
        return View();
    }

   public IActionResult Staff()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Staff(StaffVM vm, IFormFile photo)
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
                Role = vm.Role,
                Password = vm.Password,
                Status = "Active",
                Email = vm.Email,
                Otp = 1, //TO DO
                Path = hp.SavePhoto(photo, "staff"),
            });
            db.SaveChanges();
            TempData["Info"] = $"Staff {vm.Name} inserted.";
        }
        return RedirectToAction("Staff");
    }
}
