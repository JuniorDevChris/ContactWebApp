using System.ComponentModel.DataAnnotations;

namespace ContactAppWeb.Models
{
    public class ContactModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^1?[0-9]{10}$", ErrorMessage = "Please enter a valid 10-digit telephone number.")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        public string? UserId { get; set; } 
    }
}
