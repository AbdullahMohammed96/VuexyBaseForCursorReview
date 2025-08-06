using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VuexyBase.Application.Application.Common.Results;
using VuexyBase.Application.Application.ViewModels.Region;
using VuexyBase.Application.Application.ViewModels.City;

namespace VuexyBase.Application.Application.Contracts.Application.Dashboard.City
{
    public interface ICityService
    {
        Task<ListResultViewModel<ListCityViewModel>> GetAllCities();

        Task<bool> EditCity(UpdateCityViewModel cityVM);
        Task<bool> CreateCity(AddCityViewModel city);
        Task<bool> DeleteCity(int id);

        Task<bool> ChangeStatus(int id);

        Task<ListResultViewModel<ListCityViewModel>> FilterCities(int length, string search, string status);

        List<SelectListItem> GetAllRegions();
    }
}