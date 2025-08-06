using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VuexyBase.Application.Application.ViewModels.SubCategory
{
    public class ListSubCategoryViewModel
    {
        public int CategoryNumber { get; set; }
        public int ActiveNumber { get; set; }
        public int DisabledNumber { get; set; }
        public List<SubCategorisList> SubCategories { get; set; } = new List<SubCategorisList>();
      
    }

    public class SubCategorisList
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public int CategoryId { get; set; }


    }

}
