using FileDownload.DTO;
using FileDownload.Service;
using FileDownload.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileDownload.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        private readonly IFIleServiceInterface _fileService;
        public FileController(IFIleServiceInterface fileService)
        {
            _fileService = fileService;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _fileService.GetAllFiles();
            if (result.Any())
            {
                return View(result);
            }
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [RequestSizeLimit(209715200)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateFIleDto createFile)
        {
            try
            {

                if (createFile.Image != null && createFile.Image.Length > 209715200)
                    return BadRequest(new { Message = "File size exceeds the allowed limit." });

                if (ModelState.IsValid)
                {
                    if (createFile == null) return BadRequest(new { Message = "Error! Invalid create file request" });
                    var result = await _fileService.CreateFile(createFile);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    return BadRequest(new { Message = "Unexpected Error! Fail to create file", Error = result.Errors.Select(e => e.Description) });
                }
                else
                {
                    return View(createFile);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Interval Server Error! Failed To Create File", ex });
            }
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFile(Guid fileId)
        {
            try
            {
                if (fileId == Guid.Empty) return BadRequest(new { Message = "File not found" });
                var result = await _fileService.DeleteFile(fileId);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                return BadRequest(new { Message = "Unexpected Error! Failed to Remove FIle", Error = result.Errors.Select(e => e.Description) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error! Failed to remove file", ex });
            }
        }
        public async Task<IActionResult> DownloadFile(Guid fileId)
        {
            try
            {
                var fileResponse = await _fileService.DownloadFile(fileId);

                if (fileResponse == null)
                    return BadRequest(new { Message = "File not found." });

                return File(fileResponse.FileContent, fileResponse.ContentType, fileResponse.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error! Failed to Download FIle", ex });
            }

        }

    }
}
