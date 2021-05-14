using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileSharingSite.Models
{
    public class FileUploadModel
    {
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Укажите название")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Длина не 5 символов, не более 30.")]
        public string fileName { get; set; }
        [Display(Name = "Аннотация")]
        [StringLength(100, ErrorMessage = "Длина не может превышать 100 символов.")]
        public string Annotation { get; set; }
        public SelectList Catalogs { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать каталог")]
        public string CatalogName { get; set; }
        public string filePath { get; set; }
        public int FileId { get; set; }
    }
}
