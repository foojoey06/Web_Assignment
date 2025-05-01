using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using X.PagedList.Extensions;

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

    public IActionResult Category(string? name, string? sort, string? dir, int page = 1)
    {
        //Searching--------------------------------
        ViewBag.Name = name = name?.Trim() ?? "";

        var searched = db.Categories.Where(s => s.Name.Contains(name));

        //Sorting----------------------------------
        ViewBag.Sort = sort;
        ViewBag.Dir = dir;

        Func<Category, object> fn = sort switch
        {
            "Id" => s => s.Id,
            "Name" => s => s.Name,
            _ => s => s.Id,
        };

        var sorted = dir == "des" ?
                     searched.OrderByDescending(fn) :
                     searched.OrderBy(fn);

        //Paging-----------------------------------
        if (page < 1)
        {
            return RedirectToAction(null, new { name, sort, dir, page = 1 });
        }

        ViewBag.Categories = sorted.ToPagedList(page, 10);

        if (page > ViewBag.Categories.PageCount && ViewBag.Categories.PageCount > 0)
        {
            return RedirectToAction(null, new { name, sort, dir, page = ViewBag.Categories.PageCount });
        }
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
