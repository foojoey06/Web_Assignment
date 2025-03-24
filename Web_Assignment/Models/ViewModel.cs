using System.ComponentModel.DataAnnotations;
namespace Web_Assignment.Models;

//Category View Model
public class CategoryVM
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
}

//Staff View Model
public class StaffVM
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    [Required]
    [MaxLength(20)]
    [RegularExpression("^(Cashier|Admin|Customer)$", ErrorMessage = "Role must be 'Cashier', 'Admin' or 'Customer' only.")]
    public string Role { get; set; }
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
    public string Email { get; set; }

    //@TO DO <- OTP

}
