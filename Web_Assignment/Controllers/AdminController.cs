
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.HttpSys;
using X.PagedList.Extensions;

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

    [Authorize(Roles = "Admin")]
    public IActionResult Index(string? name, string? sort, string? dir, int page = 1)
    {

        //Searching--------------------------------------
        ViewBag.name = name = name?.Trim() ?? "";

        var searched = db.Staffs
                         .Where(s => s.Name.Contains(name))
                         .Where(s => s.Role == "Admin");

        //Sorting----------------------------------------
        ViewBag.Sort = sort;
        ViewBag.Dir = dir;

        Func<Staff, object> fn = sort switch
        {
            "Id"    => s => s.Id,
            "Name"  => s => s.Name,
            "Email" => s => s.Email,
            _       => s => s.Id,
        };

        var sorted = dir == "des" ?
                     searched.OrderByDescending(fn) :
                     searched.OrderBy(fn);

        //Paging------------------------------------------
        if (page < 1)
        {
            return RedirectToAction(null, new { name, sort, dir, page = 1 });
        }

        var m = sorted.ToPagedList(page, 10);

        if (page > m.PageCount && m.PageCount > 0)
        {
            return RedirectToAction(null, new { name, sort, dir, page = m.PageCount });
        }

        return View(m);

    }

    [Authorize(Roles = "Admin")]
    public IActionResult StaffAdd()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    //validate Emails in View Model
    public bool CheckEmail(string email)
    {
        return !db.Staffs.Any(s => s.Email == email);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult StaffAdd(StaffVM vm, IFormFile photo)
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
                Role = "Admin",
                Password = hp.HashPassword(vm.Password),
                Status = "Active",
                Email = vm.Email,
                Otp = 1, //TO DO
                Path = "/staff/" + hp.SavePhoto(photo, "staff"),
            });
            db.SaveChanges();
            TempData["Info"] = $"Staff {vm.Name} inserted.";
        }
        return RedirectToAction("StaffAdd");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult StaffDelete(int? id)
    {
        var p = db.Staffs.Find(id);


        if(p != null)
        {
            hp.DeletePhoto(p.Path, "staff");
            db.Staffs.Remove(p);
            db.SaveChanges();

            TempData["Info"] = "Record deleted";
        }

        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin")]
    public IActionResult StaffUpdate(int? id)
    {
        var p = db.Staffs.Find(id);

        if (p == null)
        {
            TempData["Info"] = "Staff Record not found.";
            return RedirectToAction("Index");
        }

        var vm = new StaffUpdateVM
        {
            Id = p.Id,
            Name = p.Name,
            Role = p.Role,
            Email = p.Email,
            PhotoURL = p.Path,
        };

        return View(vm);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult StaffUpdate(StaffUpdateVM vm)
    {
        var p = db.Staffs.Find(vm.Id);

        if(p == null)
        {
            TempData["Info"] = "Staff Record not found.";
            return RedirectToAction("Index");
        }

        //Validate Photo
        if(vm.Path != null)
        {
            var e = hp.ValidatePhoto(vm.Path);
            if (e != "") ModelState.AddModelError("Photo", e);
        }

        //Update Record
        if (ModelState.IsValid)
        {
            p.Email = vm.Email;
            p.Role = vm.Role;

            if (vm.Path != null)
            {
                hp.DeletePhoto(p.Path, "staff");
                p.Path = "/staff/" + hp.SavePhoto(vm.Path, "staff");
            }
            db.SaveChanges();

            TempData["Info"] = "Staff Record updated.";
            return RedirectToAction("Index");
        }

        return View("StaffUpdate", vm);
    }
    
}
