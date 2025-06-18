using System.ComponentModel.DataAnnotations;

namespace FileDownload.DTO
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(40,ErrorMessage ="Name cannot exceed 40 characters")]
        public string Name { get; set; }
        [EmailAddress(ErrorMessage ="You must enter a vaild email")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Please enter the password")]
        public string Password { get; set; }
        [Required]
        [StringLength(40,ErrorMessage ="Address cannot exceed 40 characters")]
        public string Address { get; set; }
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string PhoneNumber { get; set; }
        [Required]
        [StringLength(20,ErrorMessage ="Username cannot exceed 20 characters")]
        public string Username { get; set; }
    }
}
