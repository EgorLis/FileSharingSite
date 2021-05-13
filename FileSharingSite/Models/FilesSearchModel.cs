using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace FileSharingSite.Models
{
    public class FilesSearchModel
    {
        public List<FileModel> Files { get; set; }
        public SelectList Catalogs { get; set; }
        public string CatalogName { get; set; }
        public string SearchString { get; set; }
    }
}

