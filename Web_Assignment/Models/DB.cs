using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Web_Assignment.Models;

public class DB : DbContext
{
    public DB(DbContextOptions options) : base(options) { }

    //DbSets
    public DbSet<Category> Categories { get; set; }
    public DbSet<Beverage> Beverages { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Staff> Staffs { get; set; }
    public DbSet<Payment> Payments { get; set; }
}

//Entity Classes
#nullable disable warnings

public class Category
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; }

    public List<Beverage> Beverages { get; set; } = [];
}

public class Beverage
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; }
    [Precision(4, 2)]
    public decimal Price { get; set; }
    public int stock { get; set; }

    //fk
    public int CategoryId { get; set; }
    //np
    public Category Category { get; set; }
    public List<Image> Images { get; set; } = [];
    public List<OrderItem> OrderItems { get; set; } = [];
}

public class Image
{
    [Key]
    public int Id { get; set; }
    [MaxLength(700)]
    public string Path { get; set; }

    //fk
    public int BeverageId { get; set; }
    //np
    public Beverage Beverages { get; set; }
}

public class Payment
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string CustName { get; set; }
    [MaxLength(30)]
    public string CustEmail { get; set; }
    [Precision(6, 2)]
    public decimal PaidAmount { get; set; }
    [MaxLength(10)]
    public string Method { get; set; }

    //fk
    public int OrderId { get; set; }

    //np
    public Order Order { get; set; } 
}

public class Staff
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; }
    [MaxLength(20)]
    public string Role { get; set; }
    [MaxLength(100)]
    public string Password { get; set; }
    [MaxLength(15)]
    public string Status { get; set; }
    [MaxLength(30)]
    public string Email { get; set; }
    public int Otp { get; set; }
    [MaxLength(700)]
    public string Path { get; set; }

    public List<Order> Orders { get; set; } = [];
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

    //fk
    public int BeverageId { get; set; }
    public int OrderId { get; set; }

    //np
    public Beverage Beverage { get; set; }
    public Order Order { get; set; }
}

public class Order
{
    [Key]
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }
    [Precision(6, 2)]
    public decimal Amount { get; set; }

    public int StaffId { get; set; }

    //np
    public List<OrderItem> OrderItems { get; set; } = [];
    public Staff Staff { get; set; }
    public Payment? Payment { get; set; }
}

public class Admin : Staff
{

}

public class Cashier : Staff
{
    [MaxLength(100)]
    public string Path { get; set; }
}

