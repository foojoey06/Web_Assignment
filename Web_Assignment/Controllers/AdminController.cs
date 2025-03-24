using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web_Assignment.Controllers;

public class AdminController : Controller
{
    private readonly DB db;

    public AdminController(DB db)
    {
        this.db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

   public IActionResult Staff()
    {
        ViewBag.Staffs = db.Staffs;
        return View();
    }

    [HttpPost]
    public IActionResult Index(StaffVM vm)
    {
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
                Path = "Placeholder", //TO DO
            });
            //db.SaveChanges();
            //TempData["Info"] = $"Staff {vm.Name} inserted.";
        }
        return RedirectToAction("Index");
    }
}
