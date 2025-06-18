using System.ComponentModel.DataAnnotations;

namespace FileDownload.DTO
{
    public class LoginUserDto
    {
        [EmailAddress(ErrorMessage ="Enter a vaild email")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Please enter the password")]
        public string Password { get; set; }
    }
}
