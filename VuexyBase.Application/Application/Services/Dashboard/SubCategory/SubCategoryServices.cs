using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VuexyBase.Application.Application.Common.Helpers;
using VuexyBase.Application.Application.Common.Results;
using VuexyBase.Application.Application.Contracts.Application.Dashboard.SubCategory;
using VuexyBase.Application.Application.ViewModels.Category;
using VuexyBase.Application.Application.ViewModels.SubCategory;
using VuexyBase.Application.Persistence;
using VuexyBase.Domain.Enums.Helper;

namespace VuexyBase.Application.Application.Services.Dashboard.SubCategory
{
    public class SubCategoryServices : ISubCategoryService
    {
        private readonly ApplicationDbContext _context;

        public SubCategoryServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ChangeStatus(int id)
        {
            VuexyBase.Domain.Entities.General.SubCategory subCategoryStat = await _context.SubCategories.FindAsync(id);

            if (subCategoryStat is null)
            {
                return false;
            }

            subCategoryStat.IsActive = !subCategoryStat.IsActive;
            try
            {
                _context.SubCategories.Update(subCategoryStat);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateSubCategory(AddSubCategoryViewModel subCategory)
        {
            var subCategoryEntity = new VuexyBase.Domain.Entities.General.SubCategory
            {
                NameAr = subCategory.NameAr,
                NameEn = subCategory.NameEn,
                CreationDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                IsActive = subCategory.IsActive,
                IsDeleted = false,
                CategoryId = subCategory.CategoryId ,
                
            };
            try
            {
                _context.SubCategories.Add(subCategoryEntity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteSubCategory(int id)
        {
            var subCategory = await _context.SubCategories.FindAsync(id);

            if (subCategory is null)
            {
                return false;
            }

            subCategory.IsDeleted = true;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;

            }
        }

        public async Task<bool> EditSubCategory(UpdateSubCategoryViewModel subCategoryVM)
        {
            var subCategory = await _context.SubCategories.FindAsync(subCategoryVM.Id);

            if (subCategory is null)
                return false;

            

            subCategory.NameAr = subCategoryVM.NameAr;
            subCategory.NameEn = subCategoryVM.NameEn;
            subCategory.UpdateDate = DateTime.UtcNow;
            subCategory.IsActive = subCategoryVM.IsActive;
            subCategory.CategoryId = subCategoryVM.CategoryId;


           

            try
            {
                _context.SubCategories.Update(subCategory);
                await _context.SaveChangesAsync();
                return true;

            }
            catch
            {
                return false;
            }
        }

        public async Task<ListResultViewModel<SubCategorisList>> GetAllSubCategories()
        {
            return await FilterSubCategories();
        }

        public async Task<ListResultViewModel<SubCategorisList>> FilterSubCategories(int length = 10, string search = "", string status = "")
        {
            var query = _context.SubCategories
                .Where(c => !c.IsDeleted);

            // Stats (before filtering by search or status)
            var activeNumber = await query.CountAsync(c => c.IsActive);
            var disabledNumber = await query.CountAsync(c => !c.IsActive);
            var totalNumber = await query.CountAsync();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.NameAr.Contains(search) || c.NameEn.Contains(search));
            }

            // Apply status filter
            if (!string.IsNullOrWhiteSpace(status) && bool.TryParse(status, out bool isActiveStatus))
            {
                query = query.Where(c => c.IsActive == isActiveStatus);
            }

            // Take only needed items
            var filteredSubCategories = await query
                .OrderByDescending(c => c.CreationDate)
                .Take(length)
                .Select(c => new SubCategorisList
                {
                    Id = c.Id,
                    NameAr = c.NameAr,
                    NameEn = c.NameEn,
                    IsActive = c.IsActive,
                    CreationDate = c.CreationDate,
                    UpdateDate = c.UpdateDate ,
                    CategoryId = c.CategoryId  
                })
                .ToListAsync();



            return new ListResultViewModel<SubCategorisList>
            {
                ActiveCount = activeNumber,
                InactiveCount = disabledNumber,
                TotalCount = totalNumber,
                Items = filteredSubCategories
            };
        }

        public List<SelectListItem> GetAllCategories ()
        {
            return _context.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.NameAr }).ToList();
        }

    }
}
