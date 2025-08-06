using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VuexyBase.Application.Application.ViewModels.City
{
    public class GeneralCityViewModel : BaseEntityViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Choose the main region")]
        [Required]
        public int RegionId { get; set; }
    }
}