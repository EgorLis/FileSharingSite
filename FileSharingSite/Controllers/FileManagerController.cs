using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.StaticFiles;
using FileSharingSite.Data;
using FileSharingSite.Models;
using System;
using System.Diagnostics;
using static System.Security.Cryptography.MD5;

namespace FileSharingSite.Controllers
{
    [Authorize]
    public class FileManagerController : Controller
    {

        private IWebHostEnvironment _env;
        private readonly ApplicationContext _context;

        public FileManagerController(IWebHostEnvironment env, ApplicationContext context)
        {
            _env = env;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string catalogName, string searchString)
        {
            
            // Use LINQ to get list of genres.
            IQueryable<string> genreQuery = from m in _context.Catalog
                                            orderby m.Name
                                            select m.Name;

            var files = from m in _context.File
                         select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                files = files.Where(s => s.Name.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(catalogName))
            {
                files = files.Where(x => x.Catalog.Name == catalogName);
            }


            var catalogNameVM = new FilesSearchModel
            {
                Catalogs = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Files = await files.ToListAsync()
                
            };

            foreach(var item in catalogNameVM.Files)
            {
                item.User = await _context.User.FirstOrDefaultAsync(m => m.Id == item.UserId);
                item.Catalog = await _context.Catalog.FirstOrDefaultAsync(m => m.Id == item.CatalogId);
            }

            return View(catalogNameVM);
        }

        public async Task<IActionResult> MyFiles(string catalogName, string searchString)
        {
            if (Request.Headers["Referer"].ToString() != null)
            {
                ViewData["Reffer"] = Request.Headers["Referer"].ToString();
            }
            // Use LINQ to get list of genres.
            IQueryable<string> genreQuery = from m in _context.Catalog
                                            orderby m.Name
                                            select m.Name;
            var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.Identity.Name);
            var files = from m in _context.File
                        where m.UserId == user.Id
                        select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                files = files.Where(s => s.Name.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(catalogName))
            {
                files = files.Where(x => x.Catalog.Name == catalogName);
            }

            var catalogNameVM = new FilesSearchModel
            {
                Catalogs = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Files = await files.ToListAsync()

            };

            foreach (var item in catalogNameVM.Files)
            {
                item.User = user;
                item.Catalog = await _context.Catalog.FirstOrDefaultAsync(m => m.Id == item.CatalogId);
            }

            return View(catalogNameVM);
        }


        [AllowAnonymous]
        public async Task<IActionResult> Download(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.File
                .FirstOrDefaultAsync(m => m.Id == id);
            if (file == null)
            {
                return NotFound();
            }
            var net = new System.Net.WebClient();
            var data = net.DownloadData(file.FilePath);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/octet-stream";
            var fileName = file.Name;
            return File(content, contentType, fileName);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.File
                .FirstOrDefaultAsync(m => m.Id == id);
            if (file == null)
            {
                return NotFound();
            }
            file.Catalog = await _context.Catalog
                .FirstOrDefaultAsync(m => m.Id == file.CatalogId);
            file.User = await _context.User.FirstOrDefaultAsync(m => m.Id == file.UserId);
            return View(file);
        }

        public IActionResult Create()
        {
            IQueryable<string> genreQuery = from m in _context.Catalog
                                            orderby m.Name
                                            select m.Name;
            var model = new FileUploadModel { Catalogs = new SelectList(genreQuery.Distinct().ToList()),filePath=""};
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        [RequestSizeLimit(2147483648)]
        public async Task<IActionResult> Create(IFormFile physicalFile,  FileUploadModel uploadModel)
        {
            IQueryable<string> genreQuery = from m in _context.Catalog
                                            orderby m.Name
                                            select m.Name;
            if (ModelState.IsValid)
            {
                var catalog = await _context.Catalog.FirstOrDefaultAsync(m => m.Name == uploadModel.CatalogName);
                var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.Identity.Name);
                if (physicalFile != null && uploadModel.CatalogName != null && uploadModel.fileName != null)
                {
                    var dir = CreateFilePath(physicalFile);
                    var collision = await _context.File.FirstOrDefaultAsync(m => m.FilePath == dir);
                    var file = new FileModel
                    {
                        Name = uploadModel.fileName,
                        Annotation = uploadModel.Annotation,
                        UploadDate = DateTime.Now,
                        CatalogId = catalog.Id,
                        FilePath = dir,
                        UserId = user.Id,
                        Size = GetFileSize(physicalFile)
                    };
                    if (collision == null)
                    {
                        using (var fileStream = new FileStream(dir, FileMode.Create, FileAccess.Write))
                        {
                            physicalFile.CopyTo(fileStream);
                        }
                    }
                    _context.File.Add(file);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            if(physicalFile!=null)
            {
                uploadModel.filePath = physicalFile.FileName;
            }
            uploadModel.Catalogs = new SelectList(genreQuery.Distinct().ToList());
            return View(uploadModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            IQueryable<string> genreQuery = from m in _context.Catalog
                                            orderby m.Name
                                            select m.Name;

            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.File.FindAsync(id);
            var user = await _context.User.FindAsync(file.UserId);
            var catalog = await _context.Catalog.FirstOrDefaultAsync(m => m.Id == file.CatalogId);
            file.User = user;
            if (file != null && User.Identity.Name == file.User.Login)
            {
                var fileModel = new FileUploadModel
                {
                    Annotation = file.Annotation,
                    fileName = file.Name,
                    CatalogName = catalog.Name,
                    Catalogs = new SelectList(genreQuery.Distinct().ToList()),
                    FileId = file.Id,
                    filePath = ""
                };
                return View(fileModel);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        [RequestSizeLimit(2147483648)]
        public async Task<IActionResult> Edit(IFormFile physicalFile, FileUploadModel uploadModel)
        {
            IQueryable<string> genreQuery = from m in _context.Catalog
                                            orderby m.Name
                                            select m.Name;
            if (ModelState.IsValid)
            {
                var file = await _context.File.FirstOrDefaultAsync(m => m.Id == uploadModel.FileId);
                var catalog = await _context.Catalog.FirstOrDefaultAsync(m => m.Name == uploadModel.CatalogName);
                var user = await _context.User.FirstOrDefaultAsync(m => m.Login == User.Identity.Name);
                if (uploadModel.CatalogName != null && uploadModel.fileName != null)
                {
                    if (physicalFile != null)
                    {
                        int match = 0;
                        foreach (var item in _context.File)
                        {
                            if (item.Id != file.Id && item.FilePath == file.FilePath)
                                match++;
                        }
                        if (match == 0)
                        {
                            var fileInfo = new FileInfo(file.FilePath);
                            fileInfo.Delete();
                        }
                        var dir = CreateFilePath(physicalFile);
                        var collision = await _context.File.FirstOrDefaultAsync(m => m.FilePath == dir);
                        if (collision == null)
                        {
                            using (var fileStream = new FileStream(dir, FileMode.Create, FileAccess.Write))
                            {
                                physicalFile.CopyTo(fileStream);
                            }
                        }
                        file.FilePath = dir;
                        file.Size = GetFileSize(physicalFile);
                    }
                    file.CatalogId = catalog.Id;
                    file.Name = uploadModel.fileName;
                    file.Annotation = uploadModel.Annotation;
                    _context.File.Update(file);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(MyFiles));
                }
            }
            if (physicalFile != null)
            {
                uploadModel.filePath = physicalFile.FileName;
            }
            uploadModel.Catalogs = new SelectList(genreQuery.Distinct().ToList());
            return View(uploadModel);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = await _context.File
                .FirstOrDefaultAsync(m => m.Id == id);
            if (file != null)
            {
                var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == file.UserId); 
                if(User.Identity.Name == user.Login)
                {
                    int match = 0;
                    foreach(var item in _context.File)
                    {
                        if (item.Id != file.Id && item.FilePath == file.FilePath)
                            match++;
                    }
                    if(match == 0)
                    {
                        var fileInfo = new FileInfo(file.FilePath);
                        fileInfo.Delete();
                    }
                    _context.File.Remove(file);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(MyFiles));
                }
            }
            return NotFound();
        }
        private bool FileExists(int id)
        {
            return _context.File.Any(e => e.Id == id);
        }

        private string ContentType(string FileName)
        {
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(FileName, out contentType);
            return contentType;
        }

        private string CreateFilePath(IFormFile file)
        {

            var dir = _env.ContentRootPath;
            string md5String;
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = file.OpenReadStream())
                {
                    var hash = md5.ComputeHash(stream);
                    md5String = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            dir = dir + "\\Storage\\" + md5String.Substring(0, 2) + "\\" + md5String.Substring(2, 2);
            Directory.CreateDirectory(dir);
            dir = dir + "\\" + md5String;
            return dir;
           
        }

        private string GetFileSize(IFormFile file)
        {
            float temp;
            string Size = "";
            if(file.Length < 1024)
            {
                Size = file.Length.ToString();
                Size += " Б";
            }
            if (file.Length < 1048576 && file.Length >= 1024 )
            {
                temp = file.Length / (float)1024;
                temp = (float)Math.Round(temp, 2);
                Size = temp.ToString();
                Size += " КБ";
            }
            if (file.Length < 1073741824 && file.Length >= 1048576)
            {
                temp = file.Length / (float)1024 /1024;
                temp = (float)Math.Round(temp, 2);
                Size = temp.ToString();
                Size += " МБ";
            }
            if (file.Length < 1099511627776 && file.Length >= 1073741824)
            {
                temp = file.Length / (float)1024 / 1024 / 1024;
                temp = (float)Math.Round(temp, 2);
                Size = temp.ToString();
                Size += " ГБ";
            }
            return Size;
        }
        
    }
}

