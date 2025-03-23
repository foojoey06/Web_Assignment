using System.ComponentModel.DataAnnotations;
namespace Web_Assignment.Models;

public class ViewModel
{
}

public class CategoryVM
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
}
