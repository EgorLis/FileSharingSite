using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileSharingSite.Models
{
    public class CatalogModel
    {
        public int Id { get; set; }

        [StringLength(30, MinimumLength = 4)]
        [Required]
        public string Name { get; set; }

    }
}
