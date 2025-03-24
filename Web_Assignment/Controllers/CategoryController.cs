using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web_Assignment.Controllers;

public class CategoryController : Controller
{
    private readonly DB db;

    public CategoryController(DB db)
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
        if (ModelState.IsValid) //@TO DO <- kennot duplicate category name
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
    public ActionResult Delete(int? id)
    {
        var i = db.Categories.Find(id);

        if (i != null)
        {
            db.Categories.Remove(i);
            db.SaveChanges();
            TempData["Info"] = $"Category {i.Name} deleted.";
        }
        return RedirectToAction("Category");
    }

    [HttpPost]
    public ActionResult Update(int? id, string? name)
    {

        var i = db.Categories.Find(id);

        if (i != null)
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
