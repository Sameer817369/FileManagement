using FileDownload.DTO;
using FileDownload.Models;
using Microsoft.AspNetCore.Identity;

namespace FileDownload.Service.Interface
{
    public interface IFIleServiceInterface
    {
        Task<IdentityResult> CreateFile(CreateFIleDto fileDto);
        Task<IdentityResult> DeleteFile(Guid fileId);
        Task<FileResopnseDto> DownloadFile(Guid fileId);
        Task<List<Files>> GetAllFiles();
    }
}
