using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.ComponentModel; // If you're using Stripe later

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

        var cartItems = allBeverages
            .Where(b => cart.ContainsKey(b.Id))
            .Select(b => new CartItem
            {
                Beverage = new Bev
                {
                    Id = b.Id,
                    Name = b.Name,
                    Price = b.Price,
                    CategoryName = b.Category.Name,
                    PhotoURL = b.Images.FirstOrDefault()?.Path
                },
                // Fetch the size, sugar level, and temperature from the cart (this is crucial)
                Size = cart[b.Id].Size, // Get the correct size for each product
                SugarLevel = cart[b.Id].SugarLevel, // Get the correct sugar level for each product
                Temperature = cart[b.Id].Temperature, // Get the correct temperature for each product
                Quantity = cart[b.Id].Quantity, // Get the quantity from the cart
                Subtotal = b.Price * cart[b.Id].Quantity // Calculate the subtotal for this item
            })
            .ToList();

        if (Request.IsAjax()) return PartialView("_Cart", cartItems);
        return View(cartItems);
    }

    [HttpPost]
    public IActionResult UpdateCart(int ProductId, string Size, string SugarLevel, string Temperature, int Quantity)
    {
        // Check if hp is initialized and safe to use
        var cart = hp?.GetCart() ?? new Dictionary<int, CartItem>(); // Null-conditional operator to handle null

        // Fetch the product safely with null check
        var product = db?.Beverages?.Include(b => b.Images).FirstOrDefault(b => b.Id == ProductId);
        if (product == null) return NotFound(); // Return NotFound if product is null

        if (cart.ContainsKey(ProductId))
        {
            cart[ProductId].Size = Size;
            cart[ProductId].SugarLevel = SugarLevel;
            cart[ProductId].Temperature = Temperature;
            cart[ProductId].Quantity = Quantity;
        }
        else
        {
            cart.Add(ProductId, new CartItem
            {
                Beverage = new Bev
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    CategoryName = product.Category?.Name, // Add null check for Category
                    PhotoURL = product.Images?.FirstOrDefault()?.Path // Check if Images is null
                },
                Size = Size,
                SugarLevel = SugarLevel,
                Temperature = Temperature,
                Quantity = Quantity
            });
        }

        // Set the updated cart in session
        hp.SetCart(cart);

        // Handle Referer header and redirect
        var referer = Request.Headers["Referer"].ToString();
        if (string.IsNullOrEmpty(referer))
        {
            referer = Url.Action("Index");
        }

        return Redirect(referer); // Redirect to the original page
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

        // Properly return the RedirectToAction
        return RedirectToAction("Cart");
    }


    [Authorize]
    [HttpPost]
    public IActionResult Checkout(decimal total, string staffemail, string customername, string customeremail)
    {
        // Retrieve cart from session
        var cart = hp.GetCart();
        if (cart.Count == 0)
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Cart");
        }

        // Check if Staff exists based on provided username
        var staffid = db.Staffs
            .Where(s => s.Email == staffemail)
            .Select(s => s.Id)
            .FirstOrDefault();

        if (staffid == 0) // Ensure staff ID exists (0 indicates no result found)
        {
            TempData["Error"] = "Invalid staff ID.";
            return RedirectToAction("Cart");
        }

        // Create an order object
        var order = new Order
        {
            Date = DateTime.Today.ToDateOnly(),
            Time = DateTime.Now.ToTimeOnly(),
            StaffId = staffid, // Now guaranteed to exist
            Amount = total,
        };

        // Add the order to the database and save it
        db.Orders.Add(order);
        db.SaveChanges();  // Ensure the order is saved and has a valid Id

        // Iterate over cart items and create OrderItems
        foreach (var (productId, cartItem) in cart)
        {
            var p = db.Beverages.Find(productId);
            if (p == null) continue;

            // Add OrderItem for each cart item
            order.OrderItems.Add(new OrderItem
            {
                SugarLevel = cartItem.SugarLevel,  // Use the actual sugar level from cart
                Temperature = cartItem.Temperature, // Use the temperature from cart
                Size = cartItem.Size,              // Use the size from cart
                Quantity = cartItem.Quantity,      // Use the quantity from cart
                BeverageId = productId,
                OrderId = order.Id
            });
        }

        // Now add the payment after the order has been saved and has a valid ID
        var payment = new Payment
        {
            CustEmail = customeremail,
            CustName = customername,
            PaidAmount = total,
            Method = "Card",
            OrderId = order.Id  // OrderId is now valid and set
        };

        // Add payment to the database
        db.Payments.Add(payment);
        db.SaveChanges();

        // Reset the cart after successful checkout
        hp.SetCart(null);  // Clears the cart

        // Redirect to Payment (or any post-checkout page)
        return Payment(order.Id, customeremail);
    }


    [Authorize]
    [HttpPost]
    public IActionResult Payment(int id, string customeremail)
    {
        // Obtain the order with order items and beverages. If null, cancel.
        var order = db.Orders
                      .Include(o => o.OrderItems)
                      .ThenInclude(oi => oi.Beverage) // Include the Beverage for product details
                      .FirstOrDefault(o => o.Id == id);

        if (order == null) return RedirectToAction("Cancel");

        // Obtain the total. If < 2.00, cancel.
        var total = order.OrderItems
            .Sum(oi => oi.Beverage.Price * oi.Quantity);

        if (total < 2) return RedirectToAction("Index");

        var domain = Url.Action("", "", null, "https");

        // Create a list to hold the line items
        var lineItems = new List<SessionLineItemOptions>();

        foreach (var oi in order.OrderItems)
        {
            // Create the price object for each item
            var priceOptions = new PriceCreateOptions
            {
                UnitAmount = Convert.ToInt64(oi.Beverage.Price * 100), // Convert price to cents
                Currency = "myr",
                ProductData = new PriceProductDataOptions
                {
                    Name = $"{oi.Beverage.Name} - {oi.Size} - {oi.Temperature} - {oi.SugarLevel} Sugar", // Create a name from product details
                },
            };

            var priceService = new PriceService();
            var price = priceService.Create(priceOptions);

            // Add the line item using the price id
            lineItems.Add(new SessionLineItemOptions
            {
                Price = price.Id, // Associate the price with the line item
                Quantity = oi.Quantity, // Quantity from the order item
            });
        }

        // Create the Stripe session
        var options = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = domain + "Home/Success?sessionId={CHECKOUT_SESSION_ID}",
            CancelUrl = domain + "Home/Cancel",
            CustomerEmail = customeremail,
            ClientReferenceId = order.Id.ToString(),
            LineItems = lineItems,
        };

        var sessionService = new SessionService();
        var session = sessionService.Create(options);

        // Redirect to Stripe checkout page
        return Redirect(session.Url);
    }

    [Authorize]
    public IActionResult Cancel()
    {
        return View();
    }

    [Authorize]
    public IActionResult Success(string? sessionId)
    {
        return View();
    }

}
