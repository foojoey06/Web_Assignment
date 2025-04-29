using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace Web_Assignment.Models;

//Category View Model
public class CategoryVM
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Remote("checkCategory", "Category", "Category with the same name already registered.")]
    public string Name { get; set; }
}

//Staff View Model
public class StaffVM
{ 
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(8)]
    [RegularExpression(@"^(?=.*[\W_])[a-zA-Z0-9\W_]{8}$", ErrorMessage = "Password Incorrect Format.")]
    public string Password { get; set; }
    
    [Required]
    [MaxLength(8)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Format.")]
    [Remote("CheckEmail", "Admin", ErrorMessage = "Email already Registered.")]
    public string Email { get; set; }

}

public class StaffUpdateVM
{
    public int Id { get; set; }

    //[Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(20)]
    [RegularExpression("^(Cashier|Admin)$", ErrorMessage = "Role must be 'Cashier' or 'Admin' only.")]
    public string Role { get; set; }
    
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Format.")]
    [Remote("CheckEmail", "Admin", ErrorMessage = "Email already Registered.")]
    public string Email { get; set; }
   
    //Other Properties
    public string? PhotoURL { get; set; }
    public IFormFile? Path { get; set; }
}


//Member View Model
public class MemberVM
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [MaxLength(8)]
    [RegularExpression(@"^(?=.*[\W_])[a-zA-Z0-9\W_]{8}$", ErrorMessage = "Password Incorrect Format.")]
    public string Password { get; set; }

    [Required]
    [MaxLength(8)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Format.")]
    [Remote("CheckEmail", "Admin", ErrorMessage = "Email already Registered.")]
    public string Email { get; set; }
}

public class MemberUpdateVM
{
    public int Id { get; set; }

    //[Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Format.")]
    [Remote("CheckEmail", "Admin", ErrorMessage = "Email already Registered.")]
    public string Email { get; set; }

    //Other Properties
    public string? PhotoURL { get; set; }
    public IFormFile? Path { get; set; }
}

//Login VM
public class LoginVM
{
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    [StringLength(100, MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
