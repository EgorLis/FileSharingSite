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
        [StringLength(30)]
        public string fileName { get; set; }
        [Display(Name = "Аннотация")]
        [StringLength(100)]
        public string Annotation { get; set; }
        public SelectList Catalogs { get; set; }
        public string CatalogName { get; set; }
        
        public int FileId { get; set; }
    }
}
