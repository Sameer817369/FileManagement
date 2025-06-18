using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileDownload.Models
{
    public class Files
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileCreatorId { get; set; }
        [ForeignKey("FileCreatorId")]
        public AppUser FileCreator { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string FileUrl { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
