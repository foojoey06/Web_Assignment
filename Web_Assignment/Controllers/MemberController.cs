using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using X.PagedList.Extensions;

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

        [Authorize(Roles = "Admin")]
        public IActionResult MemberAdd()
        {
            return View();
        }

        public IActionResult Index(string? name, string? sort, string? dir, int page = 1)
        {
            //Searching----------------------------
            ViewBag.Name = name = name?.Trim() ?? "";

            var searched = db.Staffs
                             .Where(s => s.Name.Contains(name))
                             .Where(s => s.Role == "Cashier");

            //Sorting-----------------------------
            ViewBag.Sort = sort;
            ViewBag.Dir = dir;

            Func<Staff, object> fn = sort switch
            {
                "Id" => s => s.Id,
                "Name" => s => s.Name,
                "Email" => s => s.Email,
                _ => s => s.Id,
            };

            var sorted = dir == "des" ?
                         searched.OrderByDescending(fn) :
                         searched.OrderBy(fn);

            //Paging-------------------------------
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
        [HttpPost]
        public IActionResult MemberAdd(MemberVM vm, IFormFile photo)
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
            return RedirectToAction("MemberAdd");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult MemberDelete(int? id)
        {
            var p = db.Staffs.Find(id);

            if (p != null)
            {
                hp.DeletePhoto(p.Path, "member");
                db.Staffs.Remove(p);
                db.SaveChanges();

                TempData["Info"] = "Record deleted";
            }
            return RedirectToAction("Index");
        }

        public IActionResult MemberUpdate(int? id)
        {
            var p = db.Staffs.Find(id);

            if (p == null)
            {
                TempData["Info"] = "Cashier Record not found.";
                return RedirectToAction("Index");
            }

            var vm = new MemberUpdateVM
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                PhotoURL = p.Path,
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult MemberUpdate(MemberUpdateVM vm)
        {
            var p = db.Staffs.Find(vm.Id);

            if (p == null)
            {
                TempData["Info"] = "Cashier Record not found.";
                return RedirectToAction("Index");
            }

            //Validate Photo
            if (vm.Path != null)
            {
                var e = hp.ValidatePhoto(vm.Path);
                if (e != "") ModelState.AddModelError("Photo", e);
            }

            //Update Record
            if (ModelState.IsValid)
            {
                p.Email = vm.Email;

                if (vm.Path != null)
                {
                    hp.DeletePhoto(p.Path, "member");
                    p.Path = "/member/" + hp.SavePhoto(vm.Path, "member");
                }
                db.SaveChanges();

                TempData["Info"] = "Cashier Record updated.";
                return RedirectToAction("Index");
            }

            return View(vm);
        }
    }
}
