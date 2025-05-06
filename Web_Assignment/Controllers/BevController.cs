using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Memory;
using System.Linq;
using X.PagedList.Extensions;

namespace Web_Assignment.Controllers
{
    public class BevController : Controller
    {
        private readonly DB db; 
        private readonly IWebHostEnvironment en;
        private readonly Helper hp;

        public BevController(IWebHostEnvironment en, DB db, Helper hp)
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

            var searched = db.Beverages
                             .Include(t => t.Category)
                             .Where(s => s.Name.Contains(name));

            //Sorting----------------------------------------
            ViewBag.Sort = sort;
            ViewBag.Dir = dir;

            Func<Beverage, object> fn = sort switch
            {
                "Id" => s => s.Id,
                "Name" => s => s.Name,
                "Price" => s => s.Price,
                "Stock" => s => s.stock,
                "Category Name" => s => s.Category.Name,
                _ => s => s.Id,
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
            ViewBag.Beverages = m;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult BevAdd()
        {

            ViewBag.Categories = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Admin")]
        public bool checkBevName(string name)
        {
            return !db.Beverages.Any(s => s.Name == name);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult BevAdd(BeverageVM vm, List<IFormFile> photo)
        {
            //Validate Photo Only
            if (ModelState.IsValid)
            {
                var e = "";
                foreach (var s in photo)
                {
                    e = hp.ValidatePhoto(s);
                }
                if (e != "") ModelState.AddModelError("photo", e);
                TempData["Info"] = $"{e}";
            }

            if (ModelState.IsValid)
            {
                
                db.Beverages.Add(new()
                {
                    Name = vm.Name,
                    Price = vm.Price,
                    stock = vm.Stock,
                    CategoryId = vm.CategoryName,
                });
                db.SaveChanges();

                //get beverage id
                var s = db.Beverages.FirstOrDefault(p => p.Name == vm.Name);

                foreach(var p in photo)
                {
                    db.Images.Add(new()
                    {
                        Path = "/Beverage/" + hp.SavePhoto(p, "Beverage"),
                        BeverageId = s.Id,
                    });
                }

                db.SaveChanges();
                TempData["Info"] = $"Beverage {vm.Name} inserted.";
            }
            return RedirectToAction("Index", "Bev");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult BevDelete(int? id)
        {

            var p = db.Beverages.Find(id);
            var images = db.Images.Where(s => s.BeverageId == p.Id);

            if (p != null)
            {
                foreach(var s in images)
                {
                    db.Images.Remove(s);
                    hp.DeletePhoto(s.Path, "Beverage");
                }
                db.Beverages.Remove(p);
                db.SaveChanges();

                TempData["Info"] = "Beverage record deleted";
            }

            return RedirectToAction("Index", "Bev");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult BevUpdate(int? id)
        {
            var b = db.Beverages.Include(x => x.Category).FirstOrDefault(x => x.Id == id);
            ViewBag.Categories = new SelectList(db.Categories, "Id", "Name");
            if(b == null)
            {
                TempData["Info"] = "Beverage Record not found.";
                return RedirectToAction("Index");
            }

            ViewBag.Images = db.Images.Where(s => s.BeverageId == id);

            var vm = new BevUpdateVM
            {
                Id = b.Id,
                Name = b.Name,
                Price = b.Price,
                Stock = b.stock,
                CategoryName = b.Category.Id,
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult BevUpdate (BevUpdateVM vm, List<IFormFile> photo)
        {
            //Validate Photo Only
            if (ModelState.IsValid)
            {
                var e = "";
                foreach (var s in photo)
                {
                    e = hp.ValidatePhoto(s);
                }
                if (e != "") ModelState.AddModelError("photo", e);
                TempData["Info"] = $"{e}";
            }

            var b = db.Beverages.Find(vm.Id);
            var img = db.Images.Where(s => s.BeverageId == vm.Id);

            if(b == null)
            {
                TempData["Info"] = "Beverage Record not found.";
                return RedirectToAction("Index", "Bev");
            }

            //Update beverage record
            if (ModelState.IsValid)
            {
                b.Name = vm.Name;
                b.Price = vm.Price;
                b.stock = vm.Stock;
                b.CategoryId = vm.CategoryName;

                //update photos if new photos uploaded
                if(photo.Count() > 0)
                {
                    //remove records from images 
                    if (img != null)
                    {
                        foreach (var s in img)
                        {
                            db.Images.Remove(s);
                            hp.DeletePhoto(s.Path, "Beverage");
                        }
                    }

                    //add new records for images
                    if (photo != null)
                    {
                        foreach (var s in photo)
                        {
                            db.Images.Add(new()
                            {
                                Path = "/Beverage/" + hp.SavePhoto(s, "Beverage"),
                                BeverageId = vm.Id,
                            });
                        }
                    }
                }
                db.SaveChanges();

                TempData["Info"] = "Beverage Record updated.";
                return RedirectToAction("Index", "Bev");
            }

            return RedirectToAction("Index", "Bev");
        }
    }
}
