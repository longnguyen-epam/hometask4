using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Domain
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? ImageUrl { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
