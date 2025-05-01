using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout; // If you're using Stripe later

namespace Web_Assignment.Controllers;

public class HomeController(DB db, Helper hp) : Controller
{
    public IActionResult Index()
    {
        ViewBag.Cart = hp.GetCart();
        var m = db.Beverages
    .Include(b => b.Category)
    .Include(b => b.Images)
    .ToList() // forces in-memory execution
    .Select(b => new Bev
    {
        Id = b.Id,
        Name = b.Name,
        Price = b.Price,
        CategoryName = b.Category.Name,
        PhotoURL = b.Images.FirstOrDefault()?.Path // now safe
    })
    .ToList();

        if (Request.IsAjax()) return PartialView("_Index", m);
        return View(m);
    }

    public IActionResult Cart()
    {
        var cart = hp.GetCart();

        var allBeverages = db.Beverages
            .Include(b => b.Category)
            .Include(b => b.Images)
            .ToList();

        var m = allBeverages
            .Where(b => cart.ContainsKey(b.Id))
            .Select(b => new CartItem
            {
                Beverage = new Bev
                {
                    Id = b.Id,
                    Name = b.Name,
                    Price = b.Price,
                    CategoryName = b.Category.Name,
                    PhotoURL = b.Images.FirstOrDefault()?.Path,
                },
                Quantity = cart[b.Id],
                Subtotal = b.Price * cart[b.Id],
            })
            .ToList();

        if (Request.IsAjax()) return PartialView("_Cart", m);
        return View(m);
    }

    [HttpPost]
    public IActionResult UpdateCart(int ProductId, int Quantity)
    {
        var cart = hp.GetCart();

        if (Quantity >= 1 && Quantity <= 100)
        {
            cart[ProductId] = Quantity;
        }
        else
        {
            cart.Remove(ProductId);
        }

        hp.SetCart(cart);

        var referer = Request.Headers.Referer.ToString();
        if (string.IsNullOrEmpty(referer))
            referer = Url.Action("Index");

        return Redirect(referer);
    }

    [HttpPost]
    public IActionResult RemoveCart(int ProductId)
    {
        var cart = hp.GetCart();

        // Check if the item exists in the cart before removing it
        if (cart.ContainsKey(ProductId))
        {
            cart.Remove(ProductId);
        }

        hp.SetCart(cart);

        var referer = Request.Headers.Referer.ToString();
        if (string.IsNullOrEmpty(referer))
            referer = Url.Action("Index");

        return Redirect(referer);
    }

}
