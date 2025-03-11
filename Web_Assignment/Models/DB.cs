using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Web_Assignment.Models;

public class DB : DbContext
{
    public DB(DbContextOptions options) : base(options) { }

    //DbSets
}

//Entity Classes
#nullable disable warnings

public class Category
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; }

    public List<Beverage> Beverages { get; set; }
}

public class Beverage
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; }
    [Range(1, 99.99)]
    public decimal Price { get; set; }
    [Range(1, 999)]
    public int stock { get; set; }

    public Category CategoryId { get; set; }
    public List<Image> Images { get; set; }
    public List<OrderItem> OrderItems { get; set; }
    public List<Cart> Cart { get; set; }
}

public class Image
{
    [Key]
    public int Id { get; set; }
    [MaxLength(700)]
    public string Path { get; set; }
    public int BeverageId { get; set; }
}

public class Voucher
{
    [Key]
    public int Id { get; set; }
    [MaxLength(10)]
    public string Code { get; set; }
    [Range(1,20)]
    public int Limit { get; set; }
    [Required]
    public DateOnly ExpiryDate { get; set; }
    [Required]
    public DateOnly ActivationDate { get; set; }
    [MaxLength(50)]
    public string Description { get; set; }
    [Range(1, 99.99)]
    public decimal MinSpend { get; set; }

    public List<Order> Orders { get; set; }
}

public class Payment
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string CustName { get; set; }
    [MaxLength(30)]
    public string CustEmail { get; set; }
    [Range(1, 9999.99)]
    public decimal PaidAmount { get; set; }
    [MaxLength(10)]
    public string Method { get; set; }

    public List<Order> Orders { get; set; }
}

public class Staff
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; }
    [MaxLength(20)]
    public string Role { get; set; }
    [MaxLength(8)]
    public string Password { get; set; }
    [MaxLength(15)]
    public string Status { get; set; }
    [MaxLength(30)]
    public string Email { get; set; }
    public int Otp { get; set; }
    [MaxLength(700)]
    public string Path { get; set; }

    public List<Order> Orders { get; set; }
    public List<Cart> Carts { get; set; }
}
public class OrderItem
{
    [Key]
    public int Id { get; set; }
    [MaxLength(10)]
    public string SugarLevel { get; set; }
    [MaxLength(4)]
    public string Temperature { get; set; }
    [MaxLength(1)]
    public string Size { get; set; }

    public int BeverageId { get; set; }
    public int OrderId { get; set; }
}

public class Order
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateOnly Date { get; set; }
    [Required]
    public TimeOnly Time { get; set; }
    [Range(1, 9999.99)]
    public decimal amount { get; set; }

    public Voucher VoucherId { get; set; }
    public Staff StaffId { get; set; }
    public Payment PaymentId { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}

public class Cart
{
    [Key]
    public int Id { get; set; }
    [MaxLength(10)]
    public string SugarLevel { get; set; }
    [MaxLength(4)]
    public string Temperature { get; set; }
    [MaxLength(1)]
    public string Size { get; set; }

    public Staff StaffId { get; set; }
    public Beverage BeverageId { get; set; }
}
