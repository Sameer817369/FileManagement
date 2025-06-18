namespace FileDownload.DTO
{
    public class CreateFIleDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}
