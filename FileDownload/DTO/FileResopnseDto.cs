namespace FileDownload.DTO
{
    public class FileResopnseDto
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string ContentType { get; set; }

    }
}
