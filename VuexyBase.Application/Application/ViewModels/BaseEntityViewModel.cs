using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VuexyBase.Application.Application.ViewModels
{
    public abstract class BaseEntityViewModel
    {
        [Required(ErrorMessage = "name ar is required")]
        public string NameAr { get; set; }


        [Required(ErrorMessage = "name en is required")]
        public string NameEn { get; set; }


        [Required(ErrorMessage = "IsActive is required")]
        public bool IsActive { get; set; }
    }
}
