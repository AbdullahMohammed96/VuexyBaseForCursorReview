using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VuexyBase.Application.Application.ViewModels.Region
{
    public class UpdateRegionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "name ar is required")]
        public string NameAr { get; set; }

        [Required(ErrorMessage = "name en is required")]
        public string NameEn { get; set; }

        public string Image { get; set; } = "ssssssssssss";

        [Required(ErrorMessage = "IsActive is required")]
        public bool IsActive { get; set; }
        public IFormFile? Cover { get; set; } = default!;

        //public DateTime CreationDate { get; set; }
        //public DateTime? UpdateDate { get; set; } = DateTime.UtcNow;
    }
}