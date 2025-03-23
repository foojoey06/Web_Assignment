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

    public IActionResult Category()
    {
        ViewBag.Categories = db.Categories;
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

    [HttpPost]
    public ActionResult DeleteCat(int? id)
    {
        var i = db.Categories.Find(id);

        if(i != null)
        {
            db.Categories.Remove(i);
            db.SaveChanges();
            TempData["Info"] = $"Category {i.Name} deleted.";
        }
        return RedirectToAction("Category");
    }

    [HttpPost]
    public ActionResult UpdateCat(int? id, string? name)
    {
        
        var i = db.Categories.Find(id);

        if(i != null)
        {
            db.Categories
              .ExecuteUpdate(s => s
              .SetProperty(i => i.Name, name)
              );
            db.SaveChanges();
            TempData["Info"] = $"Category Name {i.Name} updated.";
        }
        return RedirectToAction("Category");
    }
}
