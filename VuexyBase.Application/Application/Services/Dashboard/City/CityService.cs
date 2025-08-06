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
using VuexyBase.Application.Application.Contracts.Application.Dashboard.City;
using VuexyBase.Application.Application.ViewModels.Region;
using VuexyBase.Application.Application.ViewModels.City;
using VuexyBase.Application.Persistence;
using VuexyBase.Domain.Enums.Helper;

namespace VuexyBase.Application.Application.Services.Dashboard.City
{
    public class CityService : ICityService
    {
        private readonly ApplicationDbContext _context;

        public CityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ChangeStatus(int id)
        {
            VuexyBase.Domain.Entities.General.City cityStat = await _context.Cities.FindAsync(id);

            if (cityStat is null)
            {
                return false;
            }

            cityStat.IsActive = !cityStat.IsActive;
            try
            {
                _context.Cities.Update(cityStat);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateCity(AddCityViewModel city)
        {
            var cityEntity = new VuexyBase.Domain.Entities.General.City
            {
                NameAr = city.NameAr,
                NameEn = city.NameEn,
                CreationDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                IsActive = city.IsActive,
                IsDeleted = false,
                RegionId = city.RegionId,
            };
            try
            {
                _context.Cities.Add(cityEntity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city is null)
            {
                return false;
            }

            city.IsDeleted = true;

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

        public async Task<bool> EditCity(UpdateCityViewModel cityVM)
        {
            var city = await _context.Cities.FindAsync(cityVM.Id);

            if (city is null)
                return false;

            city.NameAr = cityVM.NameAr;
            city.NameEn = cityVM.NameEn;
            city.UpdateDate = DateTime.UtcNow;
            city.IsActive = cityVM.IsActive;
            city.RegionId = cityVM.RegionId;

            try
            {
                _context.Cities.Update(city);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ListResultViewModel<ListCityViewModel>> GetAllCities()
        {
            return await FilterCities();
        }

        public async Task<ListResultViewModel<ListCityViewModel>> FilterCities(int length = 10, string search = "", string status = "")
        {
            var query = _context.Cities
                .Include(c => c.Region)
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
            var filteredCities = await query
                .OrderByDescending(c => c.CreationDate)
                .Take(length)
                .Select(c => new ListCityViewModel
                {
                    Id = c.Id,
                    NameAr = c.NameAr,
                    NameEn = c.NameEn,
                    IsActive = c.IsActive,
                    CreationDate = c.CreationDate,
                    UpdateDate = c.UpdateDate,
                    RegionId = c.RegionId,
                    RegionNameAr = c.Region.NameAr,
                    RegionNameEn = c.Region.NameEn
                })
                .ToListAsync();

            return new ListResultViewModel<ListCityViewModel>
            {
                ActiveCount = activeNumber,
                InactiveCount = disabledNumber,
                TotalCount = totalNumber,
                Items = filteredCities
            };
        }

        public List<SelectListItem> GetAllRegions()
        {
            return _context.Regions
                .Where(r => !r.IsDeleted && r.IsActive)
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.NameAr })
                .ToList();
        }
    }
}