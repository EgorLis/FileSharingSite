using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FileSharingSite.Models
{

    public class FileModel
    {
        public int Id { get; set; }

        [Display(Name = "Название")]
        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [StringLength(100)]
        public string Annotation { get; set; }

        public int CatalogId { get; set; }

        [Display(Name = "Раздел")]
        [StringLength(30)]
        //[Required]
        public CatalogModel Catalog { get; set; }
        public int UserId { get; set; }

        [Display(Name = "Загрузил")]
        //[Required]
        public UserModel User { get; set; }

        [Display(Name = "Дата загрузки")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime UploadDate { get; set; }

        [Required]
        [StringLength(150)]
        public string FilePath { get; set; }

        [Display(Name = "Размер")]
        [StringLength(50)]
        [Required]
        public string Size { get; set; }
    }

}