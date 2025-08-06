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
using VuexyBase.Application.Application.Contracts.Application.Dashboard.Region;
using VuexyBase.Application.Application.ViewModels.Region;
using VuexyBase.Application.Persistence;
using VuexyBase.Domain.Entities.General;
using VuexyBase.Domain.Entities.Orders;
using VuexyBase.Domain.Enums.Helper;
using static System.Net.Mime.MediaTypeNames;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VuexyBase.Application.Application.Services.Dashboard.Region
{
    public class RegionService : IRegionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public RegionService(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ListResultViewModel<RegionsList>> FilterRegions(int length = 0, string search = "", string status = "")
        {
            var query = _context.Regions
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
            var filteredRegions = await query
                .OrderByDescending(c => c.CreationDate)
                .Take(length)
                .Select(c => new RegionsList
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

            return new ListResultViewModel<RegionsList>
            {
                ActiveCount = activeNumber,
                InactiveCount = disabledNumber,
                TotalCount = totalNumber,
                Items = filteredRegions
            };
        }

        public async Task<bool> EditRegion(UpdateRegionViewModel regionVM)
        {
            const int foldernumber = (int)FileName.Region;

            var region = await _context.Regions.FindAsync(regionVM.Id);
            if (region is null)
                return false;

            var hasNewCover = regionVM.Cover is not null;

            var oldcover = region.Image;

            region.NameAr = regionVM.NameAr;
            region.NameEn = regionVM.NameEn;
            region.UpdateDate = DateTime.UtcNow;
            region.IsActive = regionVM.IsActive;

            if (hasNewCover)
            {
                region.Image = UploadHelper.Upload(regionVM.Cover, foldernumber, _hostingEnvironment);
                UploadHelper.DeleteFile(oldcover, _hostingEnvironment);
            }

            try
            {
                _context.Regions.Update(region);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateRegion(AddRegionViewModel region)
        {
            const int foldernumber = (int)FileName.Region;
            var regionEntity = new VuexyBase.Domain.Entities.General.Region
            {
                NameAr = region.NameAr,
                NameEn = region.NameEn,
                CreationDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                IsActive = region.IsActive,
                IsDeleted = false,
                Image = UploadHelper.Upload(region.Cover, foldernumber, _hostingEnvironment)
            };
            try
            {
                _context.Regions.Add(regionEntity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteRegion(int id)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region is null)
            {
                return false;
            }

            region.IsDeleted = true;

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
            VuexyBase.Domain.Entities.General.Region regionStat = await _context.Regions.FindAsync(id);

            if (regionStat is null)
            {
                return false;
            }

            regionStat.IsActive = !regionStat.IsActive;
            try
            {
                _context.Regions.Update(regionStat);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}