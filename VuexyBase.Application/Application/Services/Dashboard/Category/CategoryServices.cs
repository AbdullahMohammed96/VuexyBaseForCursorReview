using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VuexyBase.Application.Application.Common.Helpers;
using VuexyBase.Application.Application.Common.Results;
using VuexyBase.Application.Application.Contracts.Application.Dashboard.Category;
using VuexyBase.Application.Application.ViewModels.Category;
using VuexyBase.Application.Persistence;
using VuexyBase.Domain.Entities.General;
using VuexyBase.Domain.Entities.Orders;
using VuexyBase.Domain.Enums.Helper;
using static System.Net.Mime.MediaTypeNames;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VuexyBase.Application.Application.Services.Dashboard.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CategoryService(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ListResultViewModel<CategorisList>> FilterCategories(int length = 0, string search = "", string status = "")
        {
            var query = _context.Categories
                .Where(c => !c.IsDeleted);

            // Stats (before filtering by search or status)
            var activeNumber = await query.CountAsync(c => c.IsActive);
            var disabledNumber = await query.CountAsync(c => !c.IsActive);
            var totalNumber = await query.CountAsync();

            // Await stat counts in parallel
            //await Task.WhenAll(activeNumberTask, disabledNumberTask, totalNumberTask);

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
            var filteredCategories = await query
                .OrderByDescending(c => c.CreationDate)
                .Take(length)
                .Select(c => new CategorisList
                {
                    Id = c.Id,
                    NameAr = c.NameAr,
                    NameEn = c.NameEn,
                    Image = c.Image,
                    IsActive = c.IsActive,
                    CreationDate = c.CreationDate,
                    UpdateDate = c.UpdateDate
                })
                .ToListAsync();


            return new ListResultViewModel<CategorisList>
            {
                ActiveCount = activeNumber,
                InactiveCount = disabledNumber,
                TotalCount = totalNumber,
                Items = filteredCategories
            };
        }

        //public async Task<ListCategoryViewModel> GetAllCategories()
        //{
        //    ListCategoryViewModel listCategoryViewModel = new ListCategoryViewModel();
        //    var categories = await _context.Categories.ToListAsync();
        //    listCategoryViewModel.ActiveNumber = categories.Count(c => (c.IsActive == true && c.IsDeleted == false));
        //    listCategoryViewModel.DisabledNumber = categories.Count(c => (c.IsActive == false && c.IsDeleted == false));
        //    listCategoryViewModel.CategoryNumber = categories.Count(c => c.IsDeleted == false);
        //    listCategoryViewModel.Categories = categories.Where(c => c.IsDeleted == false).Select(c => new CategorisList
        //    {
        //        Id = c.Id,
        //        NameAr = c.NameAr,
        //        NameEn = c.NameEn,
        //        Image = "aaa",
        //        IsActive = c.IsActive,
        //        CreationDate = c.CreationDate,
        //        UpdateDate = c.UpdateDate
        //    }).ToList();

        //    return listCategoryViewModel;

        //}
        //f
        public async Task<bool> EditCategory(UpdateCategoryViewModel categoryVM)
        {
            const int foldernumber = (int)FileName.Category;

            var category = await _context.Categories.FindAsync(categoryVM.Id);
            if (category is null)
                return false;

            var hasNewCover = categoryVM.Cover is not null;

            var oldcover = category.Image;

            category.NameAr = categoryVM.NameAr;
            category.NameEn = categoryVM.NameEn;
            category.UpdateDate = DateTime.UtcNow;
            category.IsActive = categoryVM.IsActive;
            

            if (hasNewCover)
            {
                category.Image = UploadHelper.Upload(categoryVM.Cover, foldernumber, _hostingEnvironment);
                UploadHelper.DeleteFile(oldcover, _hostingEnvironment);
            }

            try
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return true;

            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateCategory(AddCategoryViewModel category)
        {
            const int foldernumber = (int)FileName.Category;
            var categoryEntity = new VuexyBase.Domain.Entities.General.Category
            {
                NameAr = category.NameAr,
                NameEn = category.NameEn,
                CreationDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                IsActive = category.IsActive,
                IsDeleted = false,


                Image = UploadHelper.Upload(category.Cover, foldernumber, _hostingEnvironment)
            };
            try
            {
                _context.Categories.Add(categoryEntity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }

        //[HttpDelete]
        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is null)
            {
                return false;
            }

            category.IsDeleted = true;

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
        [HttpPost]
        public async Task<bool> ChangeStatus(int id)
        {
            VuexyBase.Domain.Entities.General.Category categoryStat = await _context.Categories.FindAsync(id);

            if (categoryStat is null)
            {
                return false;
            }

            categoryStat.IsActive = !categoryStat.IsActive;
            try
            {
                _context.Categories.Update(categoryStat);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //public async Task<ListCategoryViewModel> FilterCategories(int length, string search, string status)
        //{
        //    ListCategoryViewModel listCategoryViewModel = new ListCategoryViewModel();
        //    var categories = await _context.Categories.ToListAsync();
        //    listCategoryViewModel.ActiveNumber = categories.Count(c => (c.IsActive == true && c.IsDeleted == false));
        //    listCategoryViewModel.DisabledNumber = categories.Count(c => (c.IsActive == false && c.IsDeleted == false));
        //    listCategoryViewModel.CategoryNumber = categories.Count(c => c.IsDeleted == false);

        //    if (!string.IsNullOrEmpty(search))
        //        categories = categories.Where(c => c.NameAr.Contains(search) || c.NameEn.Contains(search)).ToList();
        //    if (!string.IsNullOrEmpty(status))
        //        categories = categories.Where(c => c.IsActive.ToString() == status).ToList();


        //    categories = categories.Take(length).ToList();

        //    listCategoryViewModel.Categories = categories.Where(c => c.IsDeleted == false).Select(c => new CategorisList
        //    {
        //        Id = c.Id,
        //        NameAr = c.NameAr,
        //        NameEn = c.NameEn,
        //        Image = "aaa",
        //        IsActive = c.IsActive,
        //        CreationDate = c.CreationDate,
        //        UpdateDate = c.UpdateDate
        //    }).ToList();



        //    return listCategoryViewModel;

        //}

       

    }
}
