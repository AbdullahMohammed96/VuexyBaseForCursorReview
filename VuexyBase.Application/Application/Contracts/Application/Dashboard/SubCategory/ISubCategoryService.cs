using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VuexyBase.Application.Application.Common.Results;
using VuexyBase.Application.Application.ViewModels.Category;
using VuexyBase.Application.Application.ViewModels.SubCategory;

namespace VuexyBase.Application.Application.Contracts.Application.Dashboard.SubCategory
{
    public interface ISubCategoryService
    {
        Task<ListResultViewModel<SubCategorisList>> GetAllSubCategories();

        Task<bool> EditSubCategory(UpdateSubCategoryViewModel subCategoryVM);
        Task<bool> CreateSubCategory(AddSubCategoryViewModel subCategory);
        Task<bool> DeleteSubCategory(int id);

        Task<bool> ChangeStatus(int id);

        Task<ListResultViewModel<SubCategorisList>> FilterSubCategories(int length, string search, string status);

        List<SelectListItem> GetAllCategories();
    }
}
