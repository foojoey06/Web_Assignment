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

    public bool checkCategory(string Name)
    {
        return !db.Categories.Any(s => s.Name == Name);
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

    public IActionResult CategoryUpdate(int? id)
    {
        var p = db.Categories.Find(id);

        if(p == null)
        {
            return RedirectToAction("Category");
        }

        var vm = new CategoryVM
        {
            Id = p.Id,
            Name = p.Name
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult CategoryUpdate(CategoryVM vm)
    {

        var i = db.Categories.Find(vm.Id);

        if (i != null)
        {
            i.Name = vm.Name;
            db.SaveChanges();
            TempData["Info"] = $"Category Name {i.Name} updated.";
        }
        return RedirectToAction("Category");
    }
}
