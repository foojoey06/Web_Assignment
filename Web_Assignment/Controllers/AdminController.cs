using Microsoft.AspNetCore.Mvc;

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

    public IActionResult Category()
    {
        return View();

    }
    [HttpPost]
    public IActionResult Category(CategoryVM vm)
    {
        if (ModelState.IsValid)
        {
            db.Categories.Add(new()
            {
                Name = vm.Name,
            });
            db.SaveChanges();
        }
        TempData["Info"] = $"Category {vm.Name} inserted.";
        return RedirectToAction("Category");
    }
}
