using FileDownload.Data;
using FileDownload.DTO;
using FileDownload.Models;
using FileDownload.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FileDownload.Service
{
    public class FileService : IFIleServiceInterface
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public FileService(UserManager<AppUser> userManager, IHttpContextAccessor contextAccessor, IWebHostEnvironment webHostEnvironment, ApplicationDbContext conext)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _context = conext;
        }
        public async Task<IdentityResult> CreateFile(CreateFIleDto fileDto)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUser))
                    throw new UnauthorizedAccessException("User not found");

                string? filePath = null;

                if (fileDto.Image != null && fileDto.Image.Length > 0)
                {
                    var extension = Path.GetExtension(fileDto.Image.FileName)?.ToLower();
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".zip", ".pptx" };

                    if (!allowedExtensions.Contains(extension))
                    {
                        return IdentityResult.Failed(new IdentityError { Description = "File type not supported." });
                    }

                    if (fileDto.Image.Length > 209715200) // 200MB limit
                    {
                        return IdentityResult.Failed(new IdentityError { Description = "File size exceeds the allowed limit." });
                    }

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var rootPath = _webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath;
                    var filesPath = Path.Combine(rootPath, "Files");

                    if (!Directory.Exists(filesPath))
                    {
                        Directory.CreateDirectory(filesPath);
                    }

                    var filesFullPath = Path.Combine(filesPath, fileName);
                    using var stream = new FileStream(filesFullPath, FileMode.Create);
                    await fileDto.Image.CopyToAsync(stream);

                    filePath = Path.Combine("Files", fileName);
                }
                else
                {
                    filePath = string.Empty;
                }

                var fileModel = new Files
                {
                    FileCreatorId = currentUser,
                    Title = fileDto.Title,
                    Description = fileDto.Description,
                    FileUrl = filePath
                };

                _context.Files.Add(fileModel);
                await _context.SaveChangesAsync();

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = "An unexpected error occurred while saving the file." });
            }
        }
        public async Task<List<Files>> GetAllFiles()
        {
            try
            {
                var result = await _context.Files.Include(u=>u.FileCreator).ToListAsync();
                if (result.Any())
                {
                    return result;
                }
                return new List<Files>();
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Internal Server Error! Failed to retrive file", ex);
            }
        } 

        public async Task<IdentityResult> DeleteFile(Guid fileId)
        {
            try
            {
                if (fileId == Guid.Empty) throw new UnauthorizedAccessException("File not found");
                var fileToRemove = await _context.Files.FirstOrDefaultAsync(f => f.Id == fileId);
                if (fileToRemove != null)
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath, fileToRemove.FileUrl.TrimStart('\\'));
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                    _context.Files.Remove(fileToRemove);
                    await _context.SaveChangesAsync();
                    return IdentityResult.Success;
                }
                return IdentityResult.Failed(new IdentityError { Description = "Failed to remove file" });
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Internal Server Error! Failed to delete file", ex);
            }

        }
        public async Task<FileResopnseDto> DownloadFile(Guid fileId)
        {
            var fileToDownload = await _context.Files.FirstOrDefaultAsync(f => f.Id == fileId);
            if (fileToDownload == null || string.IsNullOrEmpty(fileToDownload.FileUrl))
                throw new InvalidOperationException("File not found");
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath, fileToDownload.FileUrl.TrimStart('\\'));
            if (!File.Exists(filePath))
                throw new InvalidOperationException("File does not exist");
            var contentType = GetContentType(filePath);
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            return new FileResopnseDto
            {
                FileName = Path.GetFileName(filePath),
                FileContent = fileBytes,
                ContentType = contentType
            };
        }
        private static string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
