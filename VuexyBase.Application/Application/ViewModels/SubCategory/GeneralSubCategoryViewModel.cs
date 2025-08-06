using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VuexyBase.Application.Application.ViewModels.SubCategory
{
    public  class GeneralSubCategoryViewModel : BaseEntityViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Choose the main category")]
        [Required]
        public int CategoryId { get; set; }
    }
}
