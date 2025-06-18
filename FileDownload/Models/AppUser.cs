using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FileDownload.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
